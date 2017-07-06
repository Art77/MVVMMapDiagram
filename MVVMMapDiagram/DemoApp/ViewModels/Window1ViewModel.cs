using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DiagramDesigner.Helpers;
using DiagramDesigner;
using System.ComponentModel;
using System.Windows.Data;
using MapDiagram.Persistence.Common;
using System.Threading.Tasks;

namespace MapDiagram
{
    public class Window1ViewModel : INPCBase
    {

        private List<int> savedDiagrams = new List<int>();
        private int? savedDiagramId;
        private List<SelectableDesignerItemViewModelBase> itemsToRemove;
        private IMessageBoxService messageBoxService;
        private IDatabaseAccessService databaseAccessService;
        private DiagramViewModel diagramViewModel = new DiagramViewModel();
        private bool isBusy = false;


        public Window1ViewModel()
        {
            messageBoxService = ApplicationServicesProvider.Instance.Provider.MessageBoxService;
            databaseAccessService = ApplicationServicesProvider.Instance.Provider.DatabaseAccessService;

            foreach (var savedDiagram in databaseAccessService.FetchAllDiagram())
            {
                savedDiagrams.Add(savedDiagram.Id);
            }

            ToolBoxViewModel = new ToolBoxViewModel();
            DiagramViewModel = new DiagramViewModel();

            DeleteSelectedItemsCommand = new SimpleCommand(ExecuteDeleteSelectedItemsCommand);
            CreateNewDiagramCommand = new SimpleCommand(ExecuteCreateNewDiagramCommand);
            SaveDiagramCommand = new SimpleCommand(ExecuteSaveDiagramCommand);
            LoadDiagramCommand = new SimpleCommand(ExecuteLoadDiagramCommand);
        }

        public SimpleCommand DeleteSelectedItemsCommand { get; private set; }
        public SimpleCommand CreateNewDiagramCommand { get; private set; }
        public SimpleCommand SaveDiagramCommand { get; private set; }
        public SimpleCommand LoadDiagramCommand { get; private set; }
        public ToolBoxViewModel ToolBoxViewModel { get; private set; }


        public DiagramViewModel DiagramViewModel
        {
            get
            {
                return diagramViewModel;
            }
            set
            {
                if (diagramViewModel != value)
                {
                    diagramViewModel = value;
                    NotifyChanged("DiagramViewModel");
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    NotifyChanged("IsBusy");
                }
            }
        }


        public List<int> SavedDiagrams
        {
            get
            {
                return savedDiagrams;
            }
            set
            {
                if (savedDiagrams != value)
                {
                    savedDiagrams = value;
                    NotifyChanged("SavedDiagrams");
                }
            }
        }

        public int? SavedDiagramId
        {
            get
            {
                return savedDiagramId;
            }
            set
            {
                if (savedDiagramId != value)
                {
                    savedDiagramId = value;
                    NotifyChanged("SavedDiagramId");
                }
            }
        }



        private void ExecuteDeleteSelectedItemsCommand(object parameter)
        {
            itemsToRemove = DiagramViewModel.SelectedItems;
            List<SelectableDesignerItemViewModelBase> connectionsToAlsoRemove = new List<SelectableDesignerItemViewModelBase>();

            foreach (var connector in DiagramViewModel.Items.OfType<ConnectorViewModel>())
            {
                if (ItemsToDeleteHasConnector(itemsToRemove, connector.SourceConnectorInfo))
                {
                    connectionsToAlsoRemove.Add(connector);
                }

                if (ItemsToDeleteHasConnector(itemsToRemove, (FullyCreatedConnectorInfo)connector.SinkConnectorInfo))
                {
                    connectionsToAlsoRemove.Add(connector);
                }

            }
            itemsToRemove.AddRange(connectionsToAlsoRemove);
            foreach (var selectedItem in itemsToRemove)
            {
                DiagramViewModel.RemoveItemCommand.Execute(selectedItem);
            }
        }

        private void ExecuteCreateNewDiagramCommand(object parameter)
        { 
            itemsToRemove = new List<SelectableDesignerItemViewModelBase>();
            SavedDiagramId = null;
            DiagramViewModel.CreateNewDiagramCommand.Execute(null);
        }

        private void ExecuteSaveDiagramCommand(object parameter)
        {
            if (!DiagramViewModel.Items.Any())
            {
                messageBoxService.ShowError("There must be at least one item in order save a diagram");
                return;
            }

            IsBusy = true;
            DiagramItem wholeDiagramToSave = null;

            Task<int> task = Task.Factory.StartNew<int>(() =>
                {

                    if (SavedDiagramId != null)
                    {
                        int currentSavedDiagramId = (int)SavedDiagramId.Value;
                        wholeDiagramToSave = databaseAccessService.FetchDiagram(currentSavedDiagramId);

                        //If we have a saved diagram, we need to make sure we clear out all the removed items that
                        //the user deleted as part of this work sesssion
                        foreach (var itemToRemove in itemsToRemove)
                        {
                            DeleteFromDatabase(wholeDiagramToSave, itemToRemove);
                        }
                        //start with empty collections of connections and items, which will be populated based on current diagram
                        wholeDiagramToSave.ConnectionIds = new List<int>();
                        wholeDiagramToSave.DesignerItems = new List<DiagramItemData>();
                    }
                    else
                    {
                        wholeDiagramToSave = new DiagramItem();
                    }

                    //ensure that itemsToRemove is cleared ready for any new changes within a session
                    itemsToRemove = new List<SelectableDesignerItemViewModelBase>();

                    //Save all InternalNodeItemViewModel
                    foreach (var internalItemVM in DiagramViewModel.Items.OfType<InternalNodeItemViewModel>())
                    {
                        InternalNodeItem internalNodeItem = 
                        new InternalNodeItem(internalItemVM.Id, internalItemVM.Left, internalItemVM.Top, 
                           internalItemVM.ItemHeight,  internalItemVM.ItemWidth, internalItemVM.Angle, internalItemVM.HostUrl);
                        internalItemVM.Id = databaseAccessService.SaveInternalNodeItem(internalNodeItem);
                        wholeDiagramToSave.DesignerItems.Add(new DiagramItemData(internalNodeItem.Id, typeof(InternalNodeItem)));
                    }

                    //Save all LaneItemViewModel
                    foreach (var laneItemVM in DiagramViewModel.Items.OfType<LaneItemViewModel>())
                    {
                        LaneItem laneItem = 
                        new LaneItem(laneItemVM.Id, laneItemVM.Left, laneItemVM.Top, 
                            laneItemVM.ItemHeight, laneItemVM.ItemWidth, laneItemVM.Angle, laneItemVM.HostUrl);
                        laneItemVM.Id = databaseAccessService.SaveLaneItem(laneItem);
                        wholeDiagramToSave.DesignerItems.Add(new DiagramItemData(laneItem.Id, typeof(LaneItem)));
                    }

                    //Save all LineGrouItemViewModel
                    foreach (var laneGroupItemVM in DiagramViewModel.Items.OfType<LineGrouItemViewModel>())
                    {
                        LineGroupItem laneItem =
                        new LineGroupItem(laneGroupItemVM.Id, laneGroupItemVM.Left, laneGroupItemVM.Top,
                            laneGroupItemVM.ItemHeight, laneGroupItemVM.ItemWidth, laneGroupItemVM.Angle, laneGroupItemVM.HostUrl);
                        laneGroupItemVM.Id = databaseAccessService.SaveLineGroupItem(laneItem);
                        wholeDiagramToSave.DesignerItems.Add(new DiagramItemData(laneItem.Id, typeof(LineGroupItem)));
                    }

                    //Save all TrafficLightItemViewModel
                    foreach (var trafficLightItemVM in DiagramViewModel.Items.OfType<TrafficLightItemViewModel>())
                    {
                        TrafficLightItem trafficLightItem = 
                        new TrafficLightItem(trafficLightItemVM.Id, trafficLightItemVM.Left, trafficLightItemVM.Top, 
                        trafficLightItemVM.ItemHeight, trafficLightItemVM.ItemWidth, trafficLightItemVM.Angle, trafficLightItemVM.HostUrl);
                        trafficLightItemVM.Id = databaseAccessService.SaveTrafficLightItem(trafficLightItem);
                        wholeDiagramToSave.DesignerItems.Add(new DiagramItemData(trafficLightItem.Id, typeof(TrafficLightItem)));
                    }


                    //Save all ODConnectorItemViewModel
                    foreach (var odConnectorItemVM in DiagramViewModel.Items.OfType<ODconnectorItemViewModel>())
                    {
                        ODConnectorItem odConnectorItem = 
                        new ODConnectorItem(odConnectorItemVM.Id, odConnectorItemVM.Left, odConnectorItemVM.Top,
                        odConnectorItemVM.ItemWidth, odConnectorItemVM.ItemHeight, odConnectorItemVM.Angle, odConnectorItemVM.HostUrl);
                        odConnectorItemVM.Id = databaseAccessService.SaveODConnectorItem(odConnectorItem);
                        wholeDiagramToSave.DesignerItems.Add(new DiagramItemData(odConnectorItem.Id, typeof(ODConnectorItem)));
                    }

 
                    //Save all connections which should now have their Connection.DataItems filled in with correct Ids
                    foreach (var connectionVM in DiagramViewModel.Items.OfType<ConnectorViewModel>())
                    {
                        FullyCreatedConnectorInfo sinkConnector = connectionVM.SinkConnectorInfo as FullyCreatedConnectorInfo;

                        Connection connection = new Connection(
                            connectionVM.Id,
                            connectionVM.SourceConnectorInfo.DataItem.Id,
                            GetOrientationFromConnector(connectionVM.SourceConnectorInfo.Orientation),
                            GetTypeOfDiagramItem(connectionVM.SourceConnectorInfo.DataItem),
                            sinkConnector.DataItem.Id,
                            GetOrientationFromConnector(sinkConnector.Orientation),
                            GetTypeOfDiagramItem(sinkConnector.DataItem));

                        connectionVM.Id = databaseAccessService.SaveConnection(connection);
                        wholeDiagramToSave.ConnectionIds.Add(connectionVM.Id);
                    }

                    wholeDiagramToSave.Id = databaseAccessService.SaveDiagram(wholeDiagramToSave);
                    return wholeDiagramToSave.Id;
                });
            task.ContinueWith((ant) =>
            {
                int wholeDiagramToSaveId = ant.Result;
                if (!savedDiagrams.Contains(wholeDiagramToSaveId))
                {
                    List<int> newDiagrams = new List<int>(savedDiagrams);
                    newDiagrams.Add(wholeDiagramToSaveId);
                    SavedDiagrams = newDiagrams;

                }
                IsBusy = false;
                messageBoxService.ShowInformation(string.Format("Finished saving Diagram Id : {0}", wholeDiagramToSaveId));

            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void ExecuteLoadDiagramCommand(object parameter)
        {
            IsBusy = true;
            DiagramItem wholeDiagramToLoad = null;
            if (SavedDiagramId == null)
            {
                messageBoxService.ShowError("You need to select a diagram to load");
                return;
            }

            Task<DiagramViewModel> task = Task.Factory.StartNew<DiagramViewModel>(() =>
                {
                    //ensure that itemsToRemove is cleared ready for any new changes within a session
                    itemsToRemove = new List<SelectableDesignerItemViewModelBase>();
                    DiagramViewModel diagramViewModel = new DiagramViewModel();

                    wholeDiagramToLoad = databaseAccessService.FetchDiagram((int)SavedDiagramId.Value);

                    //load diagram items
                    foreach (DiagramItemData diagramItemData in wholeDiagramToLoad.DesignerItems)
                    {

                        if (diagramItemData.ItemType == typeof(InternalNodeItem))
                        {
                            InternalNodeItem internalNodeItem = databaseAccessService.FetchInternaNodeItem(diagramItemData.ItemId);
                            InternalNodeItemViewModel internaNodeItemViewModel =
                                new InternalNodeItemViewModel(internalNodeItem.Id, diagramViewModel, internalNodeItem.Left, internalNodeItem.Top,
                                internalNodeItem.ItemHeight, internalNodeItem.ItemWidth, internalNodeItem.Angle, internalNodeItem.HostUrl);
                            diagramViewModel.Items.Add(internaNodeItemViewModel);
                        }

                        if (diagramItemData.ItemType == typeof(LaneItem))
                        {
                            LaneItem laneItem = databaseAccessService.FetchLaneItem(diagramItemData.ItemId);
                            LaneItemViewModel laneItemViewModel =
                                new LaneItemViewModel(laneItem.Id, diagramViewModel, laneItem.Left, laneItem.Top,
                                 laneItem.ItemHeight, laneItem.ItemWidth, laneItem.Angle, laneItem.HostUrl);
                            diagramViewModel.Items.Add(laneItemViewModel);
                        }

                        if (diagramItemData.ItemType == typeof(LineGroupItem))
                        {
                            LineGroupItem lineGroupItem = databaseAccessService.FetchLineGroupItem(diagramItemData.ItemId);
                            LineGrouItemViewModel lineGroupItemViewModel =
                                new LineGrouItemViewModel(lineGroupItem.Id, diagramViewModel, lineGroupItem.Left, lineGroupItem.Top,
                                lineGroupItem.ItemHeight,lineGroupItem.ItemWidth,  lineGroupItem.Angle, lineGroupItem.HostUrl);
                            diagramViewModel.Items.Add(lineGroupItemViewModel);
                        }


                        if (diagramItemData.ItemType == typeof(ODConnectorItem))
                        {
                            ODConnectorItem odConnectorItem = databaseAccessService.FetchODConnectorItem(diagramItemData.ItemId);
                            ODconnectorItemViewModel odConnectorItemViewModel =
                                new ODconnectorItemViewModel(odConnectorItem.Id, diagramViewModel, odConnectorItem.Left, odConnectorItem.Top,
                               odConnectorItem.ItemHeight,  odConnectorItem.ItemWidth, odConnectorItem.Angle, odConnectorItem.HostUrl);
                            diagramViewModel.Items.Add(odConnectorItemViewModel);
                        }


                        if (diagramItemData.ItemType == typeof(TrafficLightItem))
                        {
                            TrafficLightItem trafficLightItem = databaseAccessService.FetchTrafficLightItem(diagramItemData.ItemId);
                            TrafficLightItemViewModel trafficLightItemViewModel =
                                new TrafficLightItemViewModel(trafficLightItem.Id, diagramViewModel, trafficLightItem.Left, trafficLightItem.Top,
                                trafficLightItem.ItemHeight, trafficLightItem.ItemWidth, trafficLightItem.Angle, trafficLightItem.HostUrl);
                            diagramViewModel.Items.Add(trafficLightItemViewModel);
                        }


                    }
                    //load connection items
                    foreach (int connectionId in wholeDiagramToLoad.ConnectionIds)
                    {
                        Connection connection = databaseAccessService.FetchConnection(connectionId);

                        DesignerItemViewModelBase sourceItem = GetConnectorDataItem(diagramViewModel, connection.SourceId, connection.SourceType);
                        ConnectorOrientation sourceConnectorOrientation = GetOrientationForConnector(connection.SourceOrientation);
                        FullyCreatedConnectorInfo sourceConnectorInfo = GetFullConnectorInfo(connection.Id, sourceItem, sourceConnectorOrientation);

                        DesignerItemViewModelBase sinkItem = GetConnectorDataItem(diagramViewModel, connection.SinkId, connection.SinkType);
                        ConnectorOrientation sinkConnectorOrientation = GetOrientationForConnector(connection.SinkOrientation);
                        FullyCreatedConnectorInfo sinkConnectorInfo = GetFullConnectorInfo(connection.Id, sinkItem, sinkConnectorOrientation);

                        ConnectorViewModel connectionVM = new ConnectorViewModel(connection.Id, diagramViewModel, sourceConnectorInfo, sinkConnectorInfo);
                        diagramViewModel.Items.Add(connectionVM);
                    }

                    return diagramViewModel;
                });
            task.ContinueWith((ant) =>
                {
                    this.DiagramViewModel = ant.Result;
                    IsBusy = false;
                    messageBoxService.ShowInformation(string.Format("Finished loading Diagram Id : {0}", wholeDiagramToLoad.Id));
 
                },TaskContinuationOptions.OnlyOnRanToCompletion);
        }


        private FullyCreatedConnectorInfo GetFullConnectorInfo(int connectorId, DesignerItemViewModelBase dataItem, ConnectorOrientation connectorOrientation)
        {
            switch(connectorOrientation)
            {
                case ConnectorOrientation.Top:
                    return dataItem.TopConnector;
                case ConnectorOrientation.Left:
                    return dataItem.LeftConnector;
                case ConnectorOrientation.Right:
                    return dataItem.RightConnector;
                case ConnectorOrientation.Bottom:
                    return dataItem.BottomConnector;

                default:
                    throw new InvalidOperationException(
                        string.Format("Found invalid persisted Connector Orientation for Connector Id: {0}", connectorId));
            }
        }

        private Type GetTypeOfDiagramItem(DesignerItemViewModelBase vmType)
        {
            if (vmType is InternalNodeItemViewModel)
                return typeof(InternalNodeItem);
            if (vmType is LaneItemViewModel)
                return typeof(LaneItem);
            if (vmType is LineGrouItemViewModel)
                return typeof(LineGroupItem);
            if (vmType is ODconnectorItemViewModel)
                return typeof(ODConnectorItem);
            if (vmType is TrafficLightItemViewModel)
                return typeof(TrafficLightItem);
           
            throw new InvalidOperationException(string.Format("Unknown diagram type"));

        }

        private DesignerItemViewModelBase GetConnectorDataItem(DiagramViewModel diagramViewModel, int conectorDataItemId, Type connectorDataItemType)
        {
            DesignerItemViewModelBase dataItem = null;

            //if (connectorDataItemType == typeof(PersistDesignerItem))
            //{
            //    dataItem = diagramViewModel.Items.OfType<PersistDesignerItemViewModel>().Single(x => x.Id == conectorDataItemId);
            //}

            if (connectorDataItemType == typeof(InternalNodeItem))
            {
                dataItem = diagramViewModel.Items.OfType<InternalNodeItemViewModel>().Single(x => x.Id == conectorDataItemId);
            }

            if (connectorDataItemType == typeof(LaneItem))
            {
                dataItem = diagramViewModel.Items.OfType<LaneItemViewModel>().Single(x => x.Id == conectorDataItemId);
            }
            if (connectorDataItemType == typeof(LineGroupItem))
            {
                dataItem = diagramViewModel.Items.OfType<LineGrouItemViewModel>().Single(x => x.Id == conectorDataItemId);
            }
            if (connectorDataItemType == typeof(ODConnectorItem))
            {
                dataItem = diagramViewModel.Items.OfType<ODconnectorItemViewModel>().Single(x => x.Id == conectorDataItemId);
            }
            if (connectorDataItemType == typeof(TrafficLightItem))
            {
                dataItem = diagramViewModel.Items.OfType<TrafficLightItemViewModel>().Single(x => x.Id == conectorDataItemId);
            }

            return dataItem;
        }


        private Orientation GetOrientationFromConnector(ConnectorOrientation connectorOrientation)
        {
            Orientation result = Orientation.None;
            switch (connectorOrientation)
            {
                case ConnectorOrientation.Bottom:
                    result = Orientation.Bottom;
                    break;
                case ConnectorOrientation.Left:
                    result = Orientation.Left;
                    break;
                case ConnectorOrientation.Top:
                    result = Orientation.Top;
                    break;
                case ConnectorOrientation.Right:
                    result = Orientation.Right;
                    break;
            }
            return result;
        }


        private ConnectorOrientation GetOrientationForConnector(Orientation persistedOrientation)
        {
            ConnectorOrientation result = ConnectorOrientation.None;
            switch (persistedOrientation)
            {
                case Orientation.Bottom:
                    result = ConnectorOrientation.Bottom;
                    break;
                case Orientation.Left:
                    result = ConnectorOrientation.Left;
                    break;
                case Orientation.Top:
                    result = ConnectorOrientation.Top;
                    break;
                case Orientation.Right:
                    result = ConnectorOrientation.Right;
                    break;
            }
            return result;
        }

        private bool ItemsToDeleteHasConnector(List<SelectableDesignerItemViewModelBase> itemsToRemove, FullyCreatedConnectorInfo connector)
        {
            return itemsToRemove.Contains(connector.DataItem);
        }



        private void DeleteFromDatabase(DiagramItem wholeDiagramToAdjust, SelectableDesignerItemViewModelBase itemToDelete)
        {

            //make sure the item is removes from Diagram as well as removing them as individual items from database
            if (itemToDelete is InternalNodeItemViewModel)
            {
                DiagramItemData diagramItemToRemoveFromParent = wholeDiagramToAdjust.DesignerItems
                    .Where(x => x.ItemId == itemToDelete.Id && x.ItemType == typeof(InternalNodeItem))
                    .Single();

                wholeDiagramToAdjust.DesignerItems.Remove(diagramItemToRemoveFromParent);
                databaseAccessService.DeleteInternalNodeItem(itemToDelete.Id);
            }
            if (itemToDelete is LaneItemViewModel)
            {
                DiagramItemData diagramItemToRemoveFromParent = wholeDiagramToAdjust.DesignerItems
                    .Where(x => x.ItemId == itemToDelete.Id && x.ItemType == typeof(LineGroupItem))
                    .Single();

                wholeDiagramToAdjust.DesignerItems.Remove(diagramItemToRemoveFromParent);
                databaseAccessService.DeleteLaneItem(itemToDelete.Id);
            }
            if (itemToDelete is LineGrouItemViewModel)
            {
                DiagramItemData diagramItemToRemoveFromParent = wholeDiagramToAdjust.DesignerItems
                    .Where(x => x.ItemId == itemToDelete.Id && x.ItemType == typeof(LineGrouItemData))
                    .Single();

                wholeDiagramToAdjust.DesignerItems.Remove(diagramItemToRemoveFromParent);
                databaseAccessService.DeleteLineGroupItem(itemToDelete.Id);
            }
            if (itemToDelete is ODconnectorItemViewModel)
            {
                DiagramItemData diagramItemToRemoveFromParent = wholeDiagramToAdjust.DesignerItems
                    .Where(x => x.ItemId == itemToDelete.Id && x.ItemType == typeof(ODConnectorItem))
                    .Single();

                wholeDiagramToAdjust.DesignerItems.Remove(diagramItemToRemoveFromParent);
                databaseAccessService.DeleteODConnectorItem(itemToDelete.Id);
            }
            if (itemToDelete is TrafficLightItemViewModel)
            {
                DiagramItemData diagramItemToRemoveFromParent = wholeDiagramToAdjust.DesignerItems
                    .Where(x => x.ItemId == itemToDelete.Id && x.ItemType == typeof(TrafficLightItem))
                    .Single();

                wholeDiagramToAdjust.DesignerItems.Remove(diagramItemToRemoveFromParent);
                databaseAccessService.DeleteTrafficLingtItem(itemToDelete.Id);
            }

            if (itemToDelete is ConnectorViewModel)
            {
                wholeDiagramToAdjust.ConnectionIds.Remove(itemToDelete.Id);
                databaseAccessService.DeleteConnection(itemToDelete.Id);
            }

            databaseAccessService.SaveDiagram(wholeDiagramToAdjust);


        }
        
    }
}

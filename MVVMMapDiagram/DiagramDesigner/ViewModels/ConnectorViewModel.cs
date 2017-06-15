using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DiagramDesigner.Helpers;
using System.Collections.ObjectModel;

namespace DiagramDesigner
{
    public class TestLineGroup : LineGroupItemViewModelBase
    {
        public TestLineGroup(int id, IDiagramViewModel parent):base(id, parent)
        {
            this.Id = id;
            this.Parent = parent;
        }
    }

    public class ConnectorViewModel : SelectableDesignerItemViewModelBase, IDiagramViewModel
    {
        private FullyCreatedConnectorInfo sourceConnectorInfo;
        private ConnectorInfoBase sinkConnectorInfo;
        private Point sourceB;
        private Point sourceA;
        private List<Point> connectionPoints;
        private Point endPoint;
        private Rect area;

        #region Connector
        public ConnectorViewModel(int id, IDiagramViewModel parent, 
            FullyCreatedConnectorInfo sourceConnectorInfo, FullyCreatedConnectorInfo sinkConnectorInfo) : base(id,parent)
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
            AddItemCommand = new SimpleCommand(ExecuteAddItemCommand);
            RemoveItemCommand = new SimpleCommand(ExecuteRemoveItemCommand);
            ClearSelectedItemsCommand = new SimpleCommand(ExecuteClearSelectedItemsCommand);
            CreateNewDiagramCommand = new SimpleCommand(ExecuteCreateNewDiagramCommand);

            this.AddItemCommand.ExecuteDelegate(new TestLineGroup(1,this));
            this.AddItemCommand.ExecuteDelegate(new TestLineGroup(2, this));

           // Mediator.Instance.Register(this);

        }

        public ConnectorViewModel(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
            AddItemCommand = new SimpleCommand(ExecuteAddItemCommand);
            RemoveItemCommand = new SimpleCommand(ExecuteRemoveItemCommand);
            ClearSelectedItemsCommand = new SimpleCommand(ExecuteClearSelectedItemsCommand);
            CreateNewDiagramCommand = new SimpleCommand(ExecuteCreateNewDiagramCommand);

            this.AddItemCommand.ExecuteDelegate(new TestLineGroup(1,this));
            this.AddItemCommand.ExecuteDelegate(new TestLineGroup(2, this));

           // Mediator.Instance.Register(this);
        }


        public static IPathFinder PathFinder { get; set; }

        public bool IsFullConnection
        {
            get { return sinkConnectorInfo is FullyCreatedConnectorInfo; }
        }

        public Point SourceA
        {
            get
            {
                return sourceA;
            }
            set
            {
                if (sourceA != value)
                {
                    sourceA = value;
                    UpdateArea();
                    NotifyChanged("SourceA");
                }
            }
        }

        public Point SourceB
        {
            get
            {
                return sourceB;
            }
            set
            {
                if (sourceB != value)
                {
                    sourceB = value;
                    UpdateArea();
                    NotifyChanged("SourceB");
                }
            }
        }

        public List<Point> ConnectionPoints
        {
            get
            {
                return connectionPoints;
            }
            private set
            {
                if (connectionPoints != value)
                {
                    connectionPoints = value;
                    NotifyChanged("ConnectionPoints");
                }
            }
        }

        public Point EndPoint
        {
            get
            {
                return endPoint;
            }
            private set
            {
                if (endPoint != value)
                {
                    endPoint = value;
                    NotifyChanged("EndPoint");
                }
            }
        }

        public Rect Area
        {
            get
            {
                return area;
            }
            private set
            {
                if (area != value)
                {
                    area = value;
                    UpdateConnectionPoints();
                    NotifyChanged("Area");
                }
            }
        }

        public ConnectorInfo ConnectorInfo(ConnectorOrientation orientation, double left, double top, Point position)
        {

            return new ConnectorInfo()
            {
                Orientation = orientation,
                DesignerItemSize = new Size(DesignerItemViewModelBase.ItemWidth, DesignerItemViewModelBase.ItemHeight),
                DesignerItemLeft = left,
                DesignerItemTop = top,
                Position = position

            };
        }

        public FullyCreatedConnectorInfo SourceConnectorInfo
        {
            get
            {
                return sourceConnectorInfo;
            }
            set
            {
                if (sourceConnectorInfo != value)
                {

                    sourceConnectorInfo = value;
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    NotifyChanged("SourceConnectorInfo");
                    (sourceConnectorInfo.DataItem as INotifyPropertyChanged).PropertyChanged += new WeakINPCEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                }
            }
        }

        public ConnectorInfoBase SinkConnectorInfo
        {
            get
            {
                return sinkConnectorInfo;
            }
            set
            {
                if (sinkConnectorInfo != value)
                {

                    sinkConnectorInfo = value;
                    if (SinkConnectorInfo is FullyCreatedConnectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector((FullyCreatedConnectorInfo)SinkConnectorInfo);
                        (((FullyCreatedConnectorInfo)sinkConnectorInfo).DataItem as INotifyPropertyChanged).PropertyChanged += new WeakINPCEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                    }
                    else
                    {

                        SourceB = ((PartCreatedConnectionInfo)SinkConnectorInfo).CurrentLocation;
                    }
                    NotifyChanged("SinkConnectorInfo");
                }
            }
        }

        private void UpdateArea()
        {
            Area = new Rect(SourceA, SourceB); 
        }

        private void UpdateConnectionPoints()
        {
            ConnectionPoints = new List<Point>()
                                   {
                                       new Point( SourceA.X  <  SourceB.X ? 0d : Area.Width, SourceA.Y  <  SourceB.Y ? 0d : Area.Height ), 
                                       new Point(SourceA.X  >  SourceB.X ? 0d : Area.Width, SourceA.Y  >  SourceB.Y ? 0d : Area.Height)
                                   };

            ConnectorInfo sourceInfo = ConnectorInfo(SourceConnectorInfo.Orientation,
                                            ConnectionPoints[0].X,
                                            ConnectionPoints[0].Y,
                                            ConnectionPoints[0]);
            var listPoint = new List<Point>();

            if(IsFullConnection)
            {
                EndPoint = ConnectionPoints.Last();
                ConnectorInfo sinkInfo = ConnectorInfo(SinkConnectorInfo.Orientation,
                                  ConnectionPoints[1].X,
                                  ConnectionPoints[1].Y,
                                  ConnectionPoints[1]);

                listPoint.Add(sourceInfo.Position);
                listPoint.Add(sinkInfo.Position);
                ConnectionPoints = listPoint;
            }
            else
            {
                listPoint.Add(sourceInfo.Position);
                listPoint.Add(ConnectionPoints[1]);
                ConnectionPoints = listPoint;
                EndPoint = new Point();
            }

            UpPosition(ConnectionPoints[0], EndPoint);
        }

        private void ConnectorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Left":
                case "Top":
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    if (this.SinkConnectorInfo is FullyCreatedConnectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector((FullyCreatedConnectorInfo)this.SinkConnectorInfo);
                    }
                    break;

            }
        }

        private void Init(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            this.Parent = sourceConnectorInfo.DataItem.Parent;
            this.SourceConnectorInfo = sourceConnectorInfo;
            this.SinkConnectorInfo = sinkConnectorInfo;
            //PathFinder = new OrthogonalPathFinder();
        }
        #endregion

        #region LineGroup

        private ObservableCollection<SelectableDesignerItemViewModelBase> items = new ObservableCollection<SelectableDesignerItemViewModelBase>();


        //[MediatorMessageSink("DoneDrawingMessage")]
        public void OnDoneDrawingMessage(bool dummy)
        {
            foreach (var item in Items.OfType<DesignerItemViewModelBase>())
            {
                item.ShowConnectors = false;
            }
        }


        public SimpleCommand AddItemCommand { get; private set; }
        public SimpleCommand RemoveItemCommand { get; private set; }
        public SimpleCommand ClearSelectedItemsCommand { get; private set; }
        public SimpleCommand CreateNewDiagramCommand { get; private set; }

        public ObservableCollection<SelectableDesignerItemViewModelBase> Items
        {
            get { return items; }
        }

        //public List<SelectableDesignerItemViewModelBase> SelectedItems
        //{
        //    get { return Items.Where(x => x.IsSelected).ToList(); }
        //}

        private void ExecuteAddItemCommand(object parameter)
        {
            if (parameter is SelectableDesignerItemViewModelBase)
            {
                SelectableDesignerItemViewModelBase item = (SelectableDesignerItemViewModelBase)parameter;
                item.Parent = this;
                items.Add(item);
            }
        }

        private void ExecuteRemoveItemCommand(object parameter)
        {
            if (parameter is SelectableDesignerItemViewModelBase)
            {
                SelectableDesignerItemViewModelBase item = (SelectableDesignerItemViewModelBase)parameter;
                items.Remove(item);
            }
        }

        private void ExecuteClearSelectedItemsCommand(object parameter)
        {
            foreach (SelectableDesignerItemViewModelBase item in Items)
            {
                item.IsSelected = false;
            }
        }

        private void ExecuteCreateNewDiagramCommand(object parameter)
        {
            Items.Clear();
        }

        private void UpPosition(Point start, Point finish)
        {
            Angle = PointHelper.AnglePoint(start, finish);
            Width = PointHelper.GetDistance(start, finish);
            Left = start.X;
            Top = start.Y - Height/2;

        }
        #endregion
        #region Notify
        private double _angle = 0;
        public double Angle
        {
            get { return _angle; }
            set
            {
                if (_angle != value)
                {
                    _angle = value;
                    NotifyChanged("Angle");
                }
              
            }
        }

        private double _height = 10;
        public double Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    NotifyChanged("Height");
                }
            }
        }

        private double _width = 10;
        public double Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    NotifyChanged("Width");
                }
            }
        }

        private double _top = 0;
        private double _left = 0;

        public double Top
        {
            get { return _top; }
            set
            {
                if (_top != value)
                {
                    _top = value;
                    NotifyChanged("Top");
                }
            }
        }
        public double Left
        {
            get { return _left; }
            set
            {
                if (_left != value)
                {
                    _left = value;
                    NotifyChanged("Left");
                }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DiagramDesigner
{
    public abstract class TwoDesignerBoxItemViewModel : DesignerItemViewModelBase
    {
        private ObservableCollection<SelectableDesignerItemViewModelBase> items = new ObservableCollection<SelectableDesignerItemViewModelBase>();

        public TwoDesignerBoxItemViewModel(int id, IDiagramViewModel parent, double left, double top) : base(id, parent, left, top)
        {
            AddItemCommand = new SimpleCommand(ExecuteAddItemCommand);
            RemoveItemCommand = new SimpleCommand(ExecuteRemoveItemCommand);
            ClearSelectedItemsCommand = new SimpleCommand(ExecuteClearSelectedItemsCommand);
            CreateNewDiagramCommand = new SimpleCommand(ExecuteCreateNewDiagramCommand);

            this.ItemHeight = 35;
            this.ItemWidth = 100;

           // Mediator.Instance.Register(this);
        }

        public TwoDesignerBoxItemViewModel():base()
        {
            AddItemCommand = new SimpleCommand(ExecuteAddItemCommand);
            RemoveItemCommand = new SimpleCommand(ExecuteRemoveItemCommand);
            ClearSelectedItemsCommand = new SimpleCommand(ExecuteClearSelectedItemsCommand);
            CreateNewDiagramCommand = new SimpleCommand(ExecuteCreateNewDiagramCommand);

            this.ItemHeight = 35;
            this.ItemWidth = 100;
            //  Mediator.Instance.Register(this);
        }

        [MediatorMessageSink("DoneLaneDrawingMessage")]
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

        public List<SelectableDesignerItemViewModelBase> SelectedItems
        {
            get { return Items.Where(x => x.IsSelected).ToList(); }
        }


        private void ExecuteAddItemCommand(object parameter)
        {
            SelectableDesignerItemViewModelBase item = new test();
            item.Parent = null;
            items.Add(item);
        }

        private void ExecuteRemoveItemCommand(object parameter)
        {

                SelectableDesignerItemViewModelBase item = items.Last();
                items.Remove(item);
            
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
      
    }

    public class test : SegmentItemViewModel
    {

    }
}

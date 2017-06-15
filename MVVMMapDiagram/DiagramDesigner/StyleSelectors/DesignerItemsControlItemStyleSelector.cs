using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DiagramDesigner
{
    public class DesignerItemsControlItemStyleSelector : StyleSelector
    {
        static DesignerItemsControlItemStyleSelector()
        {
            Instance = new DesignerItemsControlItemStyleSelector();
        }

        public static DesignerItemsControlItemStyleSelector Instance
        {
            get;
            private set;
        }


        public override Style SelectStyle(object item, DependencyObject container)
        {
            ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
            if (itemsControl == null)
                throw new InvalidOperationException("DesignerItemsControlItemStyleSelector : Could not find ItemsControl");

            if (item is InternalNodeItemViewModelBase)
            {
                return (Style)itemsControl.FindResource("twoConnectStyle");
            }

            if (item is DesignerItemViewModelBase)
            {
                return (Style)itemsControl.FindResource("fourConnectStyle");
            }

            if (item is ConnectorViewModel)
            {
                return (Style)itemsControl.FindResource("edgeStyle");
            }

            if (item is LineGroupItemViewModelBase)
            {
                return (Style)itemsControl.FindResource("lineGroupStyle");
            }

            if (item is ConnectorLineGroupViewModel)
            {
                return (Style)itemsControl.FindResource("edgeLineGroupStyle");
            }

            return null;
        }
    } 
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner.Controls
{
    public class RotateThumb : Thumb
    {
        public RotateThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.RotateThumb_DragDelta);
        }

        private Panel GetParent(DependencyObject element)
        {
            while (element != null && !((element is Panel) && ((element as Panel).Name == "selectedGrid")))
                element = VisualTreeHelper.GetParent(element);

            return element as Panel;
        }


        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            DesignerItemViewModelBase designerItem = this.DataContext as DesignerItemViewModelBase;
            foreach (DesignerItemViewModelBase item in designerItem.SelectedItems.OfType<DesignerItemViewModelBase>())
            {
                var parent = GetParent(this);
                Point currentLocation = Mouse.GetPosition(parent);
                item.Angle = PointHelper.AnglePoint(currentLocation, new Point(item.CenterX, item.CenterY))-90;
            }
        
        e.Handled = true;
      }
    }
}

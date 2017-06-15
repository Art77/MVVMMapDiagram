using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner.Controls
{
    public class DragThumb : Thumb
    {
        private Point point;
        private DesignerCanvas parent { get; set; }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            parent = null;
            Mouse.Capture(null);
        }

        private DesignerCanvas GetParent(DependencyObject element)
        {
            while (element != null && !(element is DesignerCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as DesignerCanvas;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            DesignerItemViewModelBase designerItem = this.DataContext as DesignerItemViewModelBase;
            if (e.LeftButton == MouseButtonState.Pressed)
                if (designerItem != null && designerItem.IsSelected)
                {
                    var designerItems = designerItem.SelectedItems;
                    var position = e.GetPosition(parent);
                    var delta = new Point(position.X - point.X, position.Y - point.Y);

                    foreach (DesignerItemViewModelBase item in designerItems.OfType<DesignerItemViewModelBase>())
                    {

                        item.Left += delta.X;
                        item.Top += delta.Y;
                    }
                    point = position;
                }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            parent = GetParent(this);
            point = e.GetPosition(parent);
            Mouse.Capture(this);
        }

    }
}

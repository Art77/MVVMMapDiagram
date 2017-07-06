using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DiagramDesigner
{
    public abstract class SegmentItemViewModel : DesignerItemViewModelBase
    {
        public SegmentItemViewModel(int id, IDiagramViewModel parent, double left, double top,
            double itemHeight, double ItemWidth, double angle) : base(id, parent, left, top, itemHeight, ItemWidth, angle)
        {
          
                
        }

        public SegmentItemViewModel(): base()
        {  }   
    }
}

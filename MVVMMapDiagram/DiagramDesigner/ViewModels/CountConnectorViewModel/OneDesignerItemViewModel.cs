using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DiagramDesigner
{

    public abstract class OneDesignerItemViewModel : DesignerItemViewModelBase
    {

        public OneDesignerItemViewModel(int id, DiagramViewModel parent, double left, double top,
           double itemHeight, double itemWidth, double angle, string hostUrl) : base(id, parent, left, top, itemHeight, itemWidth, angle)
        { }

        public OneDesignerItemViewModel() : base()
        { }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DiagramDesigner
{

    public abstract class TrioDesignerItemViewModel : DesignerItemViewModelBase
    {

        public TrioDesignerItemViewModel(int id, IDiagramViewModel parent, double left, double top) : base(id, parent, left, top)
        { }

        public TrioDesignerItemViewModel() : base()
        { }

    }
}

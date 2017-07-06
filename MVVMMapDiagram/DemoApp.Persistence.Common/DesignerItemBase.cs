using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapDiagram.Persistence.Common
{
    public abstract class DesignerItemBase : PersistableItemBase
    {
        public DesignerItemBase(int id, double left, double top, double itemWidth, double itemHeight,
            double angle) : base(id)
        {
            this.Left = left;
            this.Top = top;
            this.ItemWidth = itemWidth;
            this.ItemHeight = itemHeight;
            this.Angle = angle;
        }

        public double Left { get; private set; }
        public double Top { get; private set; }
        public double Angle { get; private set; }
        public double ItemWidth { get; private set; }
        public double ItemHeight { get; private set; }
        //public double Lon { get; private set; }
        //public double Lat { get; private set; }
    }
}

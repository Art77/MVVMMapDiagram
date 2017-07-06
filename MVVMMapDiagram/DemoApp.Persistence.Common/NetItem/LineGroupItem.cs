using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapDiagram.Persistence.Common
{
    public class LineGroupItem : DesignerItemBase
    {
        public LineGroupItem(int id, double left, double top, double itemHeight, double itemWidth,
            double angle, string hostUrl) : base(id, left, top, itemHeight, itemWidth, angle)
        {
            this.HostUrl = hostUrl;
        }

        public string HostUrl { get; set; }

    }
}

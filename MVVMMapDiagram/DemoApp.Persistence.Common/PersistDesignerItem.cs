﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapDiagram.Persistence.Common
{
    public class PersistDesignerItem : DesignerItemBase
    {
        public PersistDesignerItem(int id, double left, double top, double centerX, double centerY,
            double angle, string hostUrl) : base(id, left, top, centerX, centerY, angle)
        {
            this.HostUrl = hostUrl;
        }

        public string HostUrl { get; set; }

    }
}

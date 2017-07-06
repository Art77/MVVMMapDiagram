using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DiagramDesigner
{
    public class EventCorner : EventArgs
    {
        public int Tag { get; set; }
        public Point Point { get; set; }
    }
}

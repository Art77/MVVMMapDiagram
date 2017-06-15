using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DiagramDesigner
{
    public abstract class LineGroupItemViewModelBase : SelectableDesignerItemViewModelBase
    {
        private double left;
        private double top;
        private bool showConnectors = false;
        private List<FullyCreatedLineGroupConnectorInfo> connectors = new List<FullyCreatedLineGroupConnectorInfo>();

        private static double itemWidth = 65;
        private static double itemHeight = 65;

        public LineGroupItemViewModelBase(int id, IDiagramViewModel parent) : base(id, parent)
        {
            Init();
        }

        public LineGroupItemViewModelBase(): base()
        {
            Init();
        }


        public FullyCreatedLineGroupConnectorInfo LeftConnector
        {
            get { return connectors[0]; }
        }


        public FullyCreatedLineGroupConnectorInfo RightConnector
        {
            get { return connectors[1]; }
        }



        public static double ItemWidth
        {
            get { return itemWidth; }
        }

        public static double ItemHeight
        {
            get { return itemHeight; }
        }

        public bool ShowConnectors
        {
            get
            {
                return showConnectors;
            }
            set
            {
                if (showConnectors != value)
                {
                    showConnectors = value;
                    RightConnector.ShowConnectors = value;
                    LeftConnector.ShowConnectors = value;
                    NotifyChanged("ShowConnectors");
                }
            }
        }


        public double Left
        {
            get
            {
                return left;
            }
            set
            {
                if (left != value)
                {
                    left = value;
                    NotifyChanged("Left");
                }
            }
        }

        public double Top
        {
            get
            {
                return top;
            }
            set
            {
                if (top != value)
                {
                    top = value;
                    NotifyChanged("Top");
                }
            }
        }


        private void Init()
        {
            connectors.Add(new FullyCreatedLineGroupConnectorInfo(this, ConnectorOrientation.Left));
            connectors.Add(new FullyCreatedLineGroupConnectorInfo(this, ConnectorOrientation.Right));
        }
        
    }
}

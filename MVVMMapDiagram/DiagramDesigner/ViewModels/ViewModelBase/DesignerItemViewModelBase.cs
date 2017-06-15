using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DiagramDesigner
{

    public abstract class DesignerItemViewModelBase : SelectableDesignerItemViewModelBase
    {
        private double left;
        private double top;
        private bool showConnectors = false;
        private List<FullyCreatedConnectorInfo> connectors = new List<FullyCreatedConnectorInfo>();

        private double angle = 0d;
        private double itemWidth = 65;
        private double itemHeight = 65;

        public DesignerItemViewModelBase(int id, IDiagramViewModel parent, double left, double top) : base(id, parent)
        {
            this.left = left;
            this.top = top;
            Init();
        }

        public DesignerItemViewModelBase(): base()
        {
            Init();
        }


        public virtual FullyCreatedConnectorInfo TopConnector
        {
            get { return connectors[0]; }
        }


        public virtual FullyCreatedConnectorInfo BottomConnector
        {
            get { return connectors[1]; }
        }


        public virtual FullyCreatedConnectorInfo LeftConnector
        {
            get { return connectors[2]; }
        }


        public virtual FullyCreatedConnectorInfo RightConnector
        {
            get { return connectors[3]; }
        }

        public double ItemWidth
        {
            get { return itemWidth; }
            set
            {
                if(itemWidth != value)
                {
                    itemWidth = value;
                    NotifyChanged("ItemWidth");
                }
            }
        }

        public double ItemHeight
        {
            get { return itemHeight; }
            set
            {
                if(itemHeight != value)
                {
                    itemHeight = value;
                    NotifyChanged("ItemHeight");
                }
            }
        }

        public double Angle
        {
            get { return angle; }
            set
            {
                if (angle != value)
                {
                    angle = value;
                    NotifyChanged("Angle");
                }
            }
        }


        public double CenterX
        {
            get { return this.itemWidth / 2; }
        }

        public double CenterY
        {
            get { return this.itemHeight / 2; }
        }

        public virtual bool ShowConnectors
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
                    TopConnector.ShowConnectors = value;
                    BottomConnector.ShowConnectors = value;
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



        protected virtual void Init()
        {
            connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Top));
            connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Bottom));
            connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Left));
            connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Right));
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DiagramDesigner
{

    public abstract class InternalNodeItemViewModelBase : DesignerItemViewModelBase
    {
        private double left;
        private double top;
        private bool showConnectors = false;
        private List<FullyCreatedConnectorInfo> connectors = new List<FullyCreatedConnectorInfo>();

        private static double itemWidth = 30;
        private static double itemHeight = 65;

        public InternalNodeItemViewModelBase(int id, IDiagramViewModel parent, double left, double top) : base(id, parent, left, top)
        {
            this.left = left;
            this.top = top;
            Init();
        }

        public InternalNodeItemViewModelBase(): base()
        {
            Init();
        }

        public override FullyCreatedConnectorInfo LeftConnector
        {
            get { return connectors[0]; }
        }


        public override FullyCreatedConnectorInfo RightConnector
        {
            get { return connectors[1]; }
        }


        public override bool ShowConnectors
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


        protected override void Init()
        {
            connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Left));
            connectors.Add(new FullyCreatedConnectorInfo(this, ConnectorOrientation.Right));
        }
        
    }
}

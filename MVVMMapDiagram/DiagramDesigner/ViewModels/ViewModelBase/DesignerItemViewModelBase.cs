using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using System.Windows;

namespace DiagramDesigner
{


    public abstract class DesignerItemViewModelBase : SelectableDesignerItemViewModelBase
    {
        #region Private variables
        private double left;
        private double top;
        private bool showConnectors = false;
        private List<FullyCreatedConnectorInfo> connectors = new List<FullyCreatedConnectorInfo>();

        private double angle = 0d;
        private double itemWidth = 65;
        private double itemHeight = 65;

        double centerX = 0d;
        double centerY = 0d;

        double lat = 0d;
        double lon = 0d;

        private Transform transform;

        #endregion

        public delegate void SizeChangeDelegate();

        public SizeChangeDelegate SizeChange { get; set; }

        private DesignerCanvas GetDesignerCanvas(DependencyObject element)
        {
            while (element != null && !(element is DesignerCanvas))
                element = VisualTreeHelper.GetParent(element);
            return element as DesignerCanvas;
        }

        public DesignerItemViewModelBase(int id, IDiagramViewModel parent, double left, double top,
            double itemHeight, double itemWidth, double angle) : base(id, parent)
        {
            SizeChange += new SizeChangeDelegate(OnSizeChange);

            this.ItemHeight = itemHeight;
            this.ItemWidth = itemWidth;
            this.Angle = angle;
            this.left = left;
            this.top = top;
            Init();
        }



        public DesignerItemViewModelBase(): base()
        {
            SizeChange += new SizeChangeDelegate(OnSizeChange);
            Init();   
        }

        public virtual void OnSizeChange()
        {
         
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
                    CenterX = value * 0.5;
                    NotifyChanged("ItemWidth");
                    SizeChange?.Invoke();
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
                    CenterY = value * 0.5;
                    NotifyChanged("ItemHeight");
                    SizeChange?.Invoke();
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
            get { return (centerX == 0d) ? itemWidth * 0.5 : centerX; }
            set
            {
                if (centerX != value)
                {
                    centerX = value;
                    NotifyChanged("CenterX");
                }
            }
        }

        public double CenterY
        {
            get { return (centerY == 0d) ? itemHeight * 0.5 : centerY; }
            set
            {
                if (centerY != value)
                {
                    centerY = value;
                    NotifyChanged("CenterY");
                }
            }
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

        public double Latitude
        {
            get
            {
                return lat;
            }
            set
            {
                if (lat != value)
                {
                    lat = value;
                    NotifyChanged("Latitude");
                }
            }
        }

        public double Longitude
        {
            get
            {
                return lon;
            }
            set
            {
                if (lon != value)
                {
                    lon = value;
                    NotifyChanged("Longitude");
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DiagramDesigner.Helpers;
using System.Collections.ObjectModel;

namespace DiagramDesigner
{

    public class ConnectorLineGroupViewModel : SelectableDesignerItemViewModelBase
    {
        private FullyCreatedLineGroupConnectorInfo sourceConnectorInfo;
        private ConnectorInfoBase sinkConnectorInfo;
        private Point sourceB;
        private Point sourceA;
        private List<Point> connectionPoints;
        private Point endPoint;
        private Rect area;

        #region Connector
        public ConnectorLineGroupViewModel(int id, IDiagramViewModel parent,
            FullyCreatedLineGroupConnectorInfo sourceConnectorInfo, FullyCreatedLineGroupConnectorInfo sinkConnectorInfo) : base(id,parent)
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
        }

        public ConnectorLineGroupViewModel(FullyCreatedLineGroupConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
        }


        public static IPathFinder PathFinder { get; set; }

        public bool IsFullConnection
        {
            get { return sinkConnectorInfo is FullyCreatedLineGroupConnectorInfo; }
        }

        public Point SourceA
        {
            get
            {
                return sourceA;
            }
            set
            {
                if (sourceA != value)
                {
                    sourceA = value;
                    UpdateArea();
                    NotifyChanged("SourceA");
                }
            }
        }

        public Point SourceB
        {
            get
            {
                return sourceB;
            }
            set
            {
                if (sourceB != value)
                {
                    sourceB = value;
                    UpdateArea();
                    NotifyChanged("SourceB");
                }
            }
        }

        public List<Point> ConnectionPoints
        {
            get
            {
                return connectionPoints;
            }
            private set
            {
                if (connectionPoints != value)
                {
                    connectionPoints = value;
                    NotifyChanged("ConnectionPoints");
                }
            }
        }

        public Point EndPoint
        {
            get
            {
                return endPoint;
            }
            private set
            {
                if (endPoint != value)
                {
                    endPoint = value;
                    NotifyChanged("EndPoint");
                }
            }
        }

        public Rect Area
        {
            get
            {
                return area;
            }
            private set
            {
                if (area != value)
                {
                    area = value;
                    UpdateConnectionPoints();
                    NotifyChanged("Area");
                }
            }
        }

        public ConnectorInfo ConnectorInfo(ConnectorOrientation orientation, double left, double top, Point position)
        {

            return new ConnectorInfo()
            {
                Orientation = orientation,
                DesignerItemSize = new Size(DesignerItemViewModelBase.ItemWidth, DesignerItemViewModelBase.ItemHeight),
                DesignerItemLeft = left,
                DesignerItemTop = top,
                Position = position

            };
        }

        public FullyCreatedLineGroupConnectorInfo SourceConnectorInfo
        {
            get
            {
                return sourceConnectorInfo;
            }
            set
            {
                if (sourceConnectorInfo != value)
                {

                    sourceConnectorInfo = value;
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    NotifyChanged("SourceConnectorInfo");
                    (sourceConnectorInfo.DataItem as INotifyPropertyChanged).PropertyChanged += new WeakINPCEventHandler(ConnectorLineGroupViewModel_PropertyChanged).Handler;
                }
            }
        }

        public ConnectorInfoBase SinkConnectorInfo
        {
            get
            {
                return sinkConnectorInfo;
            }
            set
            {
                if (sinkConnectorInfo != value)
                {

                    sinkConnectorInfo = value;
                    if (SinkConnectorInfo is FullyCreatedLineGroupConnectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector((FullyCreatedLineGroupConnectorInfo)SinkConnectorInfo);
                        (((FullyCreatedLineGroupConnectorInfo)sinkConnectorInfo).DataItem as INotifyPropertyChanged).PropertyChanged += new WeakINPCEventHandler(ConnectorLineGroupViewModel_PropertyChanged).Handler;
                    }
                    else
                    {

                        SourceB = ((PartCreatedConnectionInfo)SinkConnectorInfo).CurrentLocation;
                    }
                    NotifyChanged("SinkConnectorInfo");
                }
            }
        }

        private void UpdateArea()
        {
            Area = new Rect(SourceA, SourceB); 
        }

        private void UpdateConnectionPoints()
        {
            ConnectionPoints = new List<Point>()
                                   {
                                       new Point( SourceA.X  <  SourceB.X ? 0d : Area.Width, SourceA.Y  <  SourceB.Y ? 0d : Area.Height ), 
                                       new Point(SourceA.X  >  SourceB.X ? 0d : Area.Width, SourceA.Y  >  SourceB.Y ? 0d : Area.Height)
                                   };

            ConnectorInfo sourceInfo = ConnectorInfo(SourceConnectorInfo.Orientation,
                                            ConnectionPoints[0].X,
                                            ConnectionPoints[0].Y,
                                            ConnectionPoints[0]);
            var listPoint = new List<Point>();

            if(IsFullConnection)
            {
                EndPoint = ConnectionPoints.Last();
                ConnectorInfo sinkInfo = ConnectorInfo(SinkConnectorInfo.Orientation,
                                  ConnectionPoints[1].X,
                                  ConnectionPoints[1].Y,
                                  ConnectionPoints[1]);

                listPoint.Add(sourceInfo.Position);
                listPoint.Add(sinkInfo.Position);
                ConnectionPoints = listPoint;
            }
            else
            {
                listPoint.Add(sourceInfo.Position);
                listPoint.Add(ConnectionPoints[1]);
                ConnectionPoints = listPoint;
                EndPoint = new Point();
            }
        }

        private void ConnectorLineGroupViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Left":
                case "Top":
                    SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
                    if (this.SinkConnectorInfo is FullyCreatedLineGroupConnectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector((FullyCreatedLineGroupConnectorInfo)this.SinkConnectorInfo);
                    }
                    break;

            }
        }

        private void Init(FullyCreatedLineGroupConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            this.Parent = sourceConnectorInfo.DataItem.Parent;
            this.SourceConnectorInfo = sourceConnectorInfo;
            this.SinkConnectorInfo = sinkConnectorInfo;
            //PathFinder = new OrthogonalPathFinder();
        }
        #endregion 
    }
}

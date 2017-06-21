using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DiagramDesigner.Helpers;
using System.Windows.Media;
using DiagramDesigner.Controls;
using GalaSoft.MvvmLight.CommandWpf;
using System;

namespace DiagramDesigner
{

    public class ConnectorViewModel : SelectableDesignerItemViewModelBase
    {
        #region Private variables
        private FullyCreatedConnectorInfo sourceConnectorInfo;
        private ConnectorInfoBase sinkConnectorInfo;
        private PointCollection points = new PointCollection();
        private Point sourceB;
        private Point sourceA;
        private Point endPoint;
        private Rect area;
        private List<Point> connectionPoints = new List<Point>();
        private List<Point> pointsSegmentPolilyne = new List<Point>();
        private bool _visibility = true;
        private double angle;
        #endregion

        public RelayCommand<PointCollection> UpPointSegmentCommand { get; private set; }

        public ConnectorViewModel(int id, IDiagramViewModel parent, 
            FullyCreatedConnectorInfo sourceConnectorInfo, FullyCreatedConnectorInfo sinkConnectorInfo) : base(id,parent)
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
            UpPointSegmentCommand = new RelayCommand<PointCollection>(OnMessagerPoint);
        }

        public ConnectorViewModel(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            Init(sourceConnectorInfo, sinkConnectorInfo);
            UpPointSegmentCommand = new RelayCommand<PointCollection>(OnMessagerPoint);
        }

        private void OnMessagerPoint(PointCollection obj)
        {
            pointsSegmentPolilyne = obj.ToList<Point>();
            var count = pointsSegmentPolilyne.Count;
            var left = pointsSegmentPolilyne[count - 2];
            var ritch = pointsSegmentPolilyne[count - 1];
            Angle = PointHelper.AnglePoint(left, ritch) + 90;
            EndPoint = PointHelper.GetMidPoint(left, ritch);
        }

        #region Property

       public List<Point> PointsSegment
        {
            get { return pointsSegmentPolilyne; }
            set
            {
                if (pointsSegmentPolilyne != value)
                {
                    pointsSegmentPolilyne = value;
                    NotifyChanged("PointsSegment");
                }
            }
        }

        public double Angle
        {
            get { return angle; }
            set
            {
                if(angle != value)
                {
                    angle = value;
                    NotifyChanged("Angle");
                }
            }
        }

        public bool IsFullConnection
        {
            get { return sinkConnectorInfo is FullyCreatedConnectorInfo; }
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
             set
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

        public ConnectorInfo ConnectorInfo(double left, double top, Point position)
        {

            return new ConnectorInfo()
            {
                DesignerItemSize = new Size(),//(DesignerItemViewModelBase.ItemWidth, DesignerItemViewModelBase.ItemHeight),
                DesignerItemLeft = left,
                DesignerItemTop = top,
                Position = position

            };
        }

        public FullyCreatedConnectorInfo SourceConnectorInfo
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
                    (sourceConnectorInfo.DataItem as INotifyPropertyChanged).PropertyChanged += 
                            new WeakINPCEventHandler(ConnectorViewModel_PropertyChanged).Handler;
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
                    if (SinkConnectorInfo is FullyCreatedConnectorInfo)
                    {
                        SourceB = PointHelper.GetPointForConnector((FullyCreatedConnectorInfo)SinkConnectorInfo);
                        (((FullyCreatedConnectorInfo)sinkConnectorInfo).DataItem as INotifyPropertyChanged).PropertyChanged += 
                            new WeakINPCEventHandler(ConnectorViewModel_PropertyChanged).Handler;
                    }
                    else
                    {
                        SourceB = ((PartCreatedConnectionInfo)SinkConnectorInfo).CurrentLocation;
                    }
                    NotifyChanged("SinkConnectorInfo");
                }
            }
        }


        public bool VisibilityConnection
        {
            get
            {
                return _visibility;
            }

            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    NotifyChanged("VisibilityEdge");
                }
            }

        }
        #endregion

        #region Method
        private void UpdateArea()
        {
            Area = new Rect(SourceA, SourceB);
        }

        private void UpdateConnectionPoints()
        {
            ConnectionPoints = new List<Point>()
                                   {                                      
                                       new Point(SourceA.X  <  SourceB.X ? 0d : Area.Width, SourceA.Y  <  SourceB.Y ? 0d : Area.Height), 
                                       new Point(SourceA.X  >  SourceB.X ? 0d : Area.Width, SourceA.Y  >  SourceB.Y ? 0d : Area.Height)
                                   };

            ConnectorInfo sourceInfo = ConnectorInfo(ConnectionPoints[0].X,
                                            ConnectionPoints[0].Y,
                                            ConnectionPoints[0]);

            var listPoint = new List<Point>();

            if (IsFullConnection)
            {
                EndPoint = PointHelper.GetMidPoint( ConnectionPoints.First(), ConnectionPoints.Last());
                ConnectorInfo sinkInfo = ConnectorInfo(ConnectionPoints[1].X,
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

        private void ConnectorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SourceA = PointHelper.GetPointForConnector(this.SourceConnectorInfo);
            if (this.SinkConnectorInfo is FullyCreatedConnectorInfo)
            {
                SourceB = PointHelper.GetPointForConnector((FullyCreatedConnectorInfo)this.SinkConnectorInfo);
            }
        }

        private void Init(FullyCreatedConnectorInfo sourceConnectorInfo, ConnectorInfoBase sinkConnectorInfo)
        {
            this.Parent = sourceConnectorInfo.DataItem.Parent;
            this.SourceConnectorInfo = sourceConnectorInfo;
            this.SinkConnectorInfo = sinkConnectorInfo;
        }

        #endregion
    }
}

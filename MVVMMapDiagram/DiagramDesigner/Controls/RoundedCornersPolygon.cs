﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiagramDesigner.Controls
{
    public enum EnumTypePoint
    {
        /// <summary>
        /// Поворотный узел
        /// </summary>
        angleConnector,
        /// <summary>
        /// Внутренние соединение (внутренний узел)
        /// </summary>
        internalNode,
        /// <summary>
        /// Группа полос
        /// </summary>
        lineGroup,
        /// <summary>
        /// Default
        /// </summary>
        None
    }

    public struct VirtualSegment
    {
        public int Id
        { get; private set; }

        public Point Ritch
        { get; set; }

        public Point Left
        { get; set; }
   
        public VirtualSegment(int id, Point Ritch, Point Left)
        {
            this.Id = id;
            this.Left = Left;
            this.Ritch = Ritch;
        }
    }

    public class RoundedCornersPolygon : Shape
    {
        private readonly Path _path;
        protected List<Path> listCornerEllipse = new List<Path>();
        private Path _selectCornerEllips;

        public event EventHandler UpSegment;

        #region DependencyProperty

        /// <summary>
        /// The <see cref="ArcRoundness" /> dependency property's name.
        /// </summary>
        public const string ArcRoundnessPropertyName = "ArcRoundness";

        /// <summary>
        /// Gets or sets the value of the <see cref="ArcRoundness" />
        /// property. This is a dependency property.
        /// </summary>
        public int ArcRoundness
        {
            get
            {
                return (int)GetValue(ArcRoundnessProperty);
            }
            set
            {
                SetValue(ArcRoundnessProperty, value);
            }
        }


        /// <summary>
        /// Identifies the <see cref="ArcRoundness" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ArcRoundnessProperty = DependencyProperty.Register(
            ArcRoundnessPropertyName,
            typeof(int),
            typeof(RoundedCornersPolygon),
            new UIPropertyMetadata(0, new PropertyChangedCallback(OnArcRoundnessCallback)));

        private static void OnArcRoundnessCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var polygon = d as RoundedCornersPolygon;
            polygon.RedrawShape();
        }

        /// <summary>
        /// The <see cref="VisibilityCorner" /> dependency property's name.
        /// </summary>
        public const string VisibilityCornerPropertyName = "VisibilityCorner";

        /// <summary>
        /// Gets or sets the value of the <see cref="VisibilityCorner" />
        /// property. This is a dependency property.
        /// </summary>
        public bool VisibilityCorner
        {
            get
            {
                return (bool)GetValue(VisibilityCornerProperty);
            }
            set
            {
                SetValue(VisibilityCornerProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="VisibilityCorner" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibilityCornerProperty = DependencyProperty.Register(
            VisibilityCornerPropertyName,
            typeof(bool),
            typeof(RoundedCornersPolygon),
            new UIPropertyMetadata(true, new PropertyChangedCallback(OnVisibilityCornerCallbak)));

        private static void OnVisibilityCornerCallbak(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var polygon = d as RoundedCornersPolygon;
            var panel = LogicalTreeHelper.GetParent(polygon) as Panel;
            var count = panel.Children.Count;
            var flag = (bool)e.NewValue;
            for (int index = 0; index < count; index++)
            {
                var child = panel.Children[index] as Path;
                if (child != null)
                {
                    child.Visibility = (flag == true) ? Visibility.Visible : Visibility.Hidden;
                }
                    
            }
        }

        /// <summary>
        /// The <see cref="ConfinesPoints" /> dependency property's name.
        /// </summary>
        public const string ConfinesPointsPropertyName = "ConfinesPoints";

        /// <summary>
        /// Gets or sets the value of the <see cref="ConfinesPoints" />
        /// property. This is a dependency property.
        /// </summary>
        public List<Point> ConfinesPoints
        {
            get
            {
                return (List<Point>)GetValue(ConfinesPointsProperty);
            }
            set
            {
                SetValue(ConfinesPointsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ConfinesPoints" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ConfinesPointsProperty = DependencyProperty.Register(
            ConfinesPointsPropertyName, typeof(List<Point>), typeof(RoundedCornersPolygon), 
            new UIPropertyMetadata(new List<Point>(), new PropertyChangedCallback(ConfinesPointsCallback)));

        private static void ConfinesPointsCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var polygon = d as RoundedCornersPolygon;
            var listPoints = e.NewValue as List<Point>;
            if ((d == null) && (listPoints == null))
                return;

            if (polygon.Points.Count == 0)
            {
                polygon.Points.Add(listPoints[0]);
                polygon.Points.Add(listPoints[1]);
                var segment = new VirtualSegment(0, listPoints[0], listPoints[1]);
                polygon.SegmentLinked.AddFirst(segment);
            }
            else
            {
                polygon.Points[0] = listPoints[0];
                var count = polygon.Points.Count;
                polygon.Points[count - 1] = listPoints[1];

                var segmentF = polygon.SegmentLinked.First.Value;
                segmentF.Ritch = polygon.Points[0];
                polygon.SegmentLinked.First.Value = segmentF;

                var segmentL = polygon.SegmentLinked.Last.Value;
                segmentL.Left = polygon.Points[1];
                polygon.SegmentLinked.Last.Value = segmentL;

                
            }
        }

        private void UpPoint()
        {
            var segmentNode = SegmentLinked.First;
            var index = 0;
            if (SegmentLinked.Count + 1 != Points.Count)
                return;
            for (int i = 0; i < SegmentLinked.Count; i++)
            {
                var segment = segmentNode.Value;
                segment.Ritch = Points[index];
                segment.Left = Points[index + 1];
                segmentNode.Value = segment;
                segmentNode = segmentNode.Next;
                index++;
            }
        }

        public LinkedList<VirtualSegment> SegmentLinked
        {
            get;
            set;
        }

        private PointCollection _points = new PointCollection();
        public PointCollection Points
        {
            get
            {
                UpSegment?.Invoke(this, EventArgs.Empty);
                return _points;
            }
            set
            {
                if(_points != value)
                    UpSegment?.Invoke(this, EventArgs.Empty);
                _points = value;
            }
        }

        private bool _isClosed;
        /// <summary>
        /// Gets or sets a value that specifies if the polygon will be closed or not.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
            set
            {
                _isClosed = value;
                
                RedrawShape();
            }
        }

        private bool _useRoundnessPercentage;
        /// <summary>
        /// Gets or sets a value that specifies if the ArcRoundness property value will be used as a percentage of the connecting segment or not.
        /// </summary>
        public bool UseRoundnessPercentage
        {
            get
            {
                return _useRoundnessPercentage;
            }
            set
            {
                _useRoundnessPercentage = value;
                RedrawShape();
            }
        }

        public Geometry Data
        {
            get
            {
                return _path.Data;
            }
        }

        public RoundedCornersPolygon()
        {
            var geometry = new PathGeometry();
            geometry.Figures.Add(new PathFigure());
            _path = new Path { Data = geometry };
            Points = new PointCollection();
            Points.Changed += Points_Changed;
            SegmentLinked = new LinkedList<VirtualSegment>();
        }

        private void Points_Changed(object sender, EventArgs e)
        {
            RedrawShape();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return _path.Data;
            }
        }
        #endregion

        #region Event
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if(Keyboard.IsKeyDown(Key.LeftCtrl) == true)
            {
                var parent = LogicalTreeHelper.GetParent(this) as Panel;
                var point = e.GetPosition(parent);
                var count = this.Points.Count;
                var listP = new PointCollection(Points);

                var index = 0;
                for (int i = 0; i < count-1; i++)
                {
                    var start = Points[index];
                    var end = Points[index + 1];
                    var flag = PointHelper.BelongLine(start, end, point);
                    if (flag == true)
                    {

                        VirtualSegment segment;
                        try
                        {
                            this.UpPoint();
                            segment = SegmentLinked.Single(w => w.Ritch == start && w.Left == end);
                        }
                        catch
                        {
                            this.UpPoint();
                            segment = SegmentLinked.Single(w => w.Ritch == start && w.Left == end);
                        }
                        var maxId = SegmentLinked.Max(m=>m.Id);
                        var newSegment = new VirtualSegment(maxId+1, start, point);
                        var nodeSegment = SegmentLinked.Find(segment);
                        SegmentLinked.AddBefore(nodeSegment, newSegment);

                        SegmentLinked.Remove(segment);

                        segment.Ritch = point;
                        var nodeUp = SegmentLinked.Find(newSegment);
                        SegmentLinked.AddAfter(nodeUp, segment);

                        index += 1;
                        break;
                    }
                    index++;
                }
                Points.Insert(index, point);

                var child = parent.Children;
                for (int i = index; i < child.Count - 1; i++)
                {
                   if (child[i] is Path)
                    {
                       int? tag = ((Path)child[i]).Tag as int?;
                       if (tag != null)
                        {
                            ((Path)child[i]).Tag = ++tag;
                        }
                    }
                }
                parent.Children.Insert(index,GetCornerEllipse(point, index));
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_selectCornerEllips == null)
                return;
        }
    

        #region Private Methods

        private Path GetCornerEllipse(Point point, int index)
        {
            var geometry = new EllipseGeometry { Center = point, RadiusX = 4 };
            geometry.RadiusY = geometry.RadiusX;
            var path = new Path { Data = geometry, Fill = Brushes.Green };
            path.MouseDown += Path_MouseDown;
            path.MouseUp += Path_MouseUp;
            path.MouseMove += Path_MouseMove;
            path.Tag = index;
            return path;
        }

        private void Path_MouseMove(object sender, MouseEventArgs e)
        {
            if ((_selectCornerEllips != null) && (e.LeftButton == MouseButtonState.Pressed))
            {
                var ellipse = _selectCornerEllips.Data as EllipseGeometry;
                int index = (int)_selectCornerEllips.Tag;
                var parent = LogicalTreeHelper.GetParent(this) as Panel;
                var point = e.GetPosition(parent);
                ellipse.Center = point;
                this.Points[index] = point;
            }
        }

        private void Path_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _selectCornerEllips = null;
            Mouse.Capture(_selectCornerEllips);
        }

        private void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var path = sender as Path;
            if(path != null)
            {
                _selectCornerEllips = path;
                Mouse.Capture(_selectCornerEllips);
            }
        }
        #endregion
        /// <summary>
        /// Redraws the entire shape.
        /// </summary>
        private void RedrawShape()
        {
            var pathGeometry = _path.Data as PathGeometry;
            if (pathGeometry == null) return;

            var pathFigure = pathGeometry.Figures[0];

            pathFigure.Segments.Clear();

            for (int counter = 0; counter < Points.Count; counter++)
            {
                switch (counter)
                {
                    case 0:
                        AddPointToPath(Points[counter], null, null);
                        break;
                    case 1:
                        AddPointToPath(Points[counter], Points[counter - 1], null);
                        break;
                    default:
                        AddPointToPath(Points[counter], Points[counter - 1], Points[counter - 2]);
                        break;
                }
            }

            if (IsClosed)
                CloseFigure(pathFigure);
        }

        /// <summary>
        /// Adds a point to the shape
        /// </summary>
        /// <param name="currentPoint">The current point added</param>
        /// <param name="prevPoint">Previous point</param>
        /// <param name="prevPrevPoint">The point before the previous point</param>
        private void AddPointToPath(Point currentPoint, Point? prevPoint, Point? prevPrevPoint)
        {
            if (Points.Count == 0)
                return;

            var pathGeometry = _path.Data as PathGeometry;
            if (pathGeometry == null) return;

            var pathFigure = pathGeometry.Figures[0];

            //the first point of a polygon
            if (prevPoint == null)
            {
                pathFigure.StartPoint = currentPoint;
            }
            //second point of the polygon, only a line will be drawn
            else if (prevPrevPoint == null)
            {
                var lines = new LineSegment { Point = currentPoint };
                pathFigure.Segments.Add(lines);
            }
            //third point and above
            else
            {
                ConnectLinePoints(pathFigure, prevPrevPoint.Value, prevPoint.Value, currentPoint, ArcRoundness, UseRoundnessPercentage);
            }
        }

        /// <summary>
        /// Adds the segments necessary to close the shape
        /// </summary>
        /// <param name="pathFigure"></param>
        private void CloseFigure(PathFigure pathFigure)
        {
            //No need to visually close the figure if we don't have at least 3 points.
            if (Points.Count < 3)
                return;
            Point backPoint, nextPoint;
            if (UseRoundnessPercentage)
            {
                backPoint = GetPointAtDistancePercent(Points[Points.Count - 1], Points[0], ArcRoundness, false);
                nextPoint = GetPointAtDistancePercent(Points[0], Points[1], ArcRoundness, true);
            }
            else
            {
                backPoint = GetPointAtDistance(Points[Points.Count - 1], Points[0], ArcRoundness, false);
                nextPoint = GetPointAtDistance(Points[0], Points[1], ArcRoundness, true);
            }
            ConnectLinePoints(pathFigure, Points[Points.Count - 2], Points[Points.Count - 1], backPoint, ArcRoundness, UseRoundnessPercentage);
            var line2 = new QuadraticBezierSegment { Point1 = Points[0], Point2 = nextPoint };
            pathFigure.Segments.Add(line2);
            pathFigure.StartPoint = nextPoint;
        }

        /// <summary>
        /// Method used to connect 2 segments with a common point, defined by 3 points and aplying an arc segment between them
        /// </summary>
        /// <param name="pathFigure"></param>
        /// <param name="p1">First point, of the first segment</param>
        /// <param name="p2">Second point, the common point</param>
        /// <param name="p3">Third point, the second point of the second segment</param>
        /// <param name="roundness">The roundness of the arc</param>
        /// <param name="usePercentage">A value that indicates if the roundness of the arc will be used as a percentage or not</param>
        private static void ConnectLinePoints(PathFigure pathFigure, Point p1, Point p2, Point p3, double roundness, bool usePercentage)
        {
            //The point on the first segment where the curve will start.
            Point backPoint;
            //The point on the second segment where the curve will end.
            Point nextPoint;
            if (usePercentage)
            {
                backPoint = GetPointAtDistancePercent(p1, p2, roundness, false);
                nextPoint = GetPointAtDistancePercent(p2, p3, roundness, true);
            }
            else
            {
                backPoint = GetPointAtDistance(p1, p2, roundness, false);
                nextPoint = GetPointAtDistance(p2, p3, roundness, true);
            }

            int lastSegmentIndex = pathFigure.Segments.Count - 1;
            //Set the ending point of the first segment.
            ((LineSegment)(pathFigure.Segments[lastSegmentIndex])).Point = backPoint;
            //Create and add the curve.
            var curve = new QuadraticBezierSegment { Point1 = p2, Point2 = nextPoint };
            pathFigure.Segments.Add(curve);
            //Create and add the new segment.
            var line = new LineSegment { Point = p3 };
            pathFigure.Segments.Add(line);
        }

        /// <summary>
        /// Gets a point on a segment, defined by two points, at a given distance.
        /// </summary>
        /// <param name="p1">First point of the segment</param>
        /// <param name="p2">Second point of the segment</param>
        /// <param name="distancePercent">Distance percent to the point</param>
        /// <param name="firstPoint">A value that indicates if the distance is calculated by the first or the second point</param>
        /// <returns></returns>
        private static Point GetPointAtDistancePercent(Point p1, Point p2, double distancePercent, bool firstPoint)
        {
            double rap = firstPoint ? distancePercent / 100 : (100 - distancePercent) / 100;
            return new Point(p1.X + (rap * (p2.X - p1.X)), p1.Y + (rap * (p2.Y - p1.Y)));
        }

        /// <summary>
        /// Gets a point on a segment, defined by two points, at a given distance.
        /// </summary>
        /// <param name="p1">First point of the segment</param>
        /// <param name="p2">Second point of the segment</param>
        /// <param name="distance">Distance  to the point</param>
        /// <param name="firstPoint">A value that indicates if the distance is calculated by the first or the second point</param>
        /// <returns>The point calculated.</returns>
        private static Point GetPointAtDistance(Point p1, Point p2, double distance, bool firstPoint)
        {
            double segmentLength = Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
            //The distance cannot be greater than half of the length of the segment
            if (distance > (segmentLength / 2))
                distance = segmentLength / 2;
            double rap = firstPoint ? distance / segmentLength : (segmentLength - distance) / segmentLength;
            return new Point(p1.X + (rap * (p2.X - p1.X)), p1.Y + (rap * (p2.Y - p1.Y)));
        }

        #endregion

    }
}
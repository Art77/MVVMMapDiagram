using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DiagramDesigner
{
    public class PointHelper
    {
        const double Rad2Deg = 180.0 / Math.PI;
        const double Deg2Rad = Math.PI / 180;

        public static Point GetPointForConnector(FullyCreatedConnectorInfo connector)
        {
            Point point = new Point();

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Top:
                    point = new Point(connector.DataItem.Left + (DesignerItemViewModelBase.ItemWidth / 2), connector.DataItem.Top - (ConnectorInfoBase.ConnectorHeight));
                    break;
                case ConnectorOrientation.Bottom:
                    point = new Point(connector.DataItem.Left + (DesignerItemViewModelBase.ItemWidth / 2), (connector.DataItem.Top + DesignerItemViewModelBase.ItemHeight) + (ConnectorInfoBase.ConnectorHeight / 2));
                    break;
                case ConnectorOrientation.Right:
                    point = new Point(connector.DataItem.Left + DesignerItemViewModelBase.ItemWidth + (ConnectorInfoBase.ConnectorWidth), connector.DataItem.Top + (DesignerItemViewModelBase.ItemHeight / 2));
                    break;
                case ConnectorOrientation.Left:
                    point = new Point(connector.DataItem.Left - ConnectorInfoBase.ConnectorWidth, connector.DataItem.Top + (DesignerItemViewModelBase.ItemHeight / 2));
                    break;
            }

            return point;
        }
        public static Point GetPointForConnector(FullyCreatedLineGroupConnectorInfo connector)
        {
            Point point = new Point();

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Right:
                    point = new Point(connector.DataItem.Left + DesignerItemViewModelBase.ItemWidth + (ConnectorInfoBase.ConnectorWidth), connector.DataItem.Top + (DesignerItemViewModelBase.ItemHeight / 2));
                    break;
                case ConnectorOrientation.Left:
                    point = new Point(connector.DataItem.Left - ConnectorInfoBase.ConnectorWidth, connector.DataItem.Top + (DesignerItemViewModelBase.ItemHeight / 2));
                    break;
            }

            return point;
        }

        //public static Point GetPointForConnector(FullyCreatedConnectorRoadInfo connector)
        //{
        //    Point point = new Point();
        //    point = new Point(connector.DataItem.Left + (DesignerItemViewModelBase.ItemWidth / 2),
        //                        connector.DataItem.Top + (DesignerItemViewModelBase.ItemHeight / 2));
        //    return point;
        //}


        public static double RadianToDegree(double angle)
        {
            return angle * Rad2Deg;
        }

        public static double DegreeToRadian(double angle)
        {
            return angle * Deg2Rad;
        }

        public static double AnglePoint(Point start, Point end)
        {
            double angle = RadianToDegree(Math.Atan2(end.X - start.X, end.Y - start.Y));
            return ((angle + Math.Ceiling(-angle / 360) * 360) - 90) * -1;
        }

        public static double GetDistance(Point p1, Point p2)
        {
            double xDelta = p1.X - p2.X;
            double yDelta = p1.Y - p2.Y;
            return Math.Sqrt(Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2));
        }

        public static Point GetMidPoint(Point p1, Point p2)
        {
            double y = (p1.Y + p2.Y) / 2;
            double x = (p1.X + p2.X) / 2;
            return new Point(x, y);
        }

        public static bool BelongLine(Point a, Point b, Point p, double n = 0.5)
        {
            double ab = GetDistance(a, b);
            double ap = GetDistance(a, p);
            double pb = GetDistance(p, b);
            double res = ap + pb;

            if (Math.Abs(ab - res) < n)
                return true;
            else
                return false;
        }

        public static Point GetCenterPoint(Point p1, double h, double w)
        {
            var p3 = new Point(p1.Y + h, p1.X);
            var p2 = new Point(p1.Y, p1.X + w);
            var p4 = new Point(p2.Y + h, p3.X + w);

            return GetMidPoint(p1, p4);
        }


    }
}

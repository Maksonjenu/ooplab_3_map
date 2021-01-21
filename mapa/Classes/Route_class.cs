using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using System.Windows.Forms;
using mapa.Classes;

namespace mapa
{
    class Route_class : MapObject
    {
        public List<PointLatLng> points = new List<PointLatLng>();
        public Route_class(string name, List<PointLatLng> Points) : base(name)
        
        {
            this.points = Points;        
        }



        public override PointLatLng getFocus() => points.Last();
        

        public override GMapMarker GetMarker()
        {
            GMapMarker marker = new GMapRoute(points)
            {
                Shape = new Path()
                {
                    Stroke = Brushes.DarkBlue, // цвет обводки
                    Fill = Brushes.DarkBlue, // цвет заливки
                    StrokeThickness = 4, // толщина обводки
                    ToolTip = objectName,
                }
            };
            return marker;
        }
       

        public override double getDist(PointLatLng point)
        {
            GeoCoordinate geo1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate geo2 = new GeoCoordinate(points[0].Lat,points[0].Lng);
            double min = geo1.GetDistanceTo(geo2);

            foreach (PointLatLng obj in points)
            {
                geo2 = new GeoCoordinate(obj.Lat, obj.Lng);
                if (geo1.GetDistanceTo(geo2) < min)
                    min = geo1.GetDistanceTo(geo2);
            }
            return min;
        }
        public override double getSquare()
        {
            return 0;
        }
    }
}

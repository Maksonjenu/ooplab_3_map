using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using mapa.Classes;

namespace mapa
{
    class Car_class : MapObject
    {
        PointLatLng point = new PointLatLng();

        public Car_class(string name,PointLatLng Point):base(name)
        {
            this.point = Point;
        }



        public override PointLatLng getFocus() => point;
        
      

        public override GMapMarker GetMarker()
        {
            GMapMarker marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Margin = new System.Windows.Thickness(-16, -16, 0, 0),
                    Width = 32, // ширина маркера
                    Height = 32, // высота маркера
                    ToolTip = objectName, // всплывающая подсказка
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/car.jpg")) // картинка
                }
            };
            return marker;
        }

       

        public override double getDist(PointLatLng point1)
        {
            GeoCoordinate geo1 = new GeoCoordinate(point.Lat, point.Lng);
            GeoCoordinate geo2 = new GeoCoordinate(point1.Lat, point1.Lng);
            return geo1.GetDistanceTo(geo2);
        }
        public override double getSquare()
        {
            return 0;
        }
    }
}

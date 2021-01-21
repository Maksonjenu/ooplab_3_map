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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         List<PointLatLng> areaspots = new List<PointLatLng>();
         List<MapObject> mapObjects = new List<MapObject>();
         List<MapObject> SortedList = new List<MapObject>();


        public MainWindow()
        {
            InitializeComponent();
            initMap();
            radioButCreate.IsChecked = true;
            createmodecombo.SelectedIndex = 0;
        }

        public void initMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Map.MapProvider = OpenStreetMapProvider.Instance;
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;
            Map.Position = new PointLatLng(55.012823, 82.950359);
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;

        }
        

        public void createMarker(List<PointLatLng> points, int index) 
        {
            MapObject mapObject = null;
            switch (index)
            {
                case 0:
                    {
                        mapObject = new Area_class(createObjectName.Text, points);
                        break;
                    }
                case 1:
                    {
                        mapObject = new Location_class(createObjectName.Text, points.Last());
                        break;
                    }
                case 2:
                    {
                        mapObject = new Car_class(createObjectName.Text, points.Last());
                        break;
                    }
                case 3:
                    {
                        mapObject = new Human_class(createObjectName.Text, points.Last());
                        break;
                    }
                case 4:
                    {
                        mapObject = new Route_class(createObjectName.Text, points);
                        break;
                    }
            
            }
            if (mapObject != null)
            {
                mapObjects.Add(mapObject);
                Map.Markers.Add(mapObject.GetMarker());
            }
        }

    

        private void MapLoaded(object sender, RoutedEventArgs e)
        {
           
        }


        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            findsresult.Items.Clear();
            PointLatLng clickedPoint = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            SortedList = mapObjects.OrderBy(o => o.getDist(clickedPoint)).ToList();
            foreach (MapObject obj in SortedList)
            {
                string mapObjectAndDistanceString = new StringBuilder()
                    .Append(obj.getTitle())
                    .Append(" - ")
                    .Append(obj.getDist(clickedPoint).ToString("0.##"))
                    .Append(" м.").ToString();
                findsresult.Items.Add(mapObjectAndDistanceString);
            }
           

        }

        private void radioButCreate_Checked(object sender, RoutedEventArgs e)
        {
            createmodecombo.IsEnabled = true;
            addbuttoncreate.IsEnabled = true;
            resetpointcreate.IsEnabled = true;
        }

        private void radioButSearch_Checked(object sender, RoutedEventArgs e)
        {
            createmodecombo.IsEnabled = false;
            addbuttoncreate.IsEnabled = false;
            resetpointcreate.IsEnabled = false;
        }

        private void addbuttoncreate_Click(object sender, RoutedEventArgs e)
        {

            if (createObjectName.Text == "")
                System.Windows.MessageBox.Show("Имя объекта пустое");
            else
            {
                createMarker(areaspots, createmodecombo.SelectedIndex);
                areaspots = new List<PointLatLng>();
                createObjectName.Clear();
            }
            checker();
            
        }

        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            double minimum = new double();
            PointLatLng clickedPoint = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            MapObject @object = null;
            if (mapObjects.Count != 0)
            minimum = mapObjects[0].getDist(clickedPoint);
            foreach (MapObject obj in mapObjects)
            {
                if (minimum > obj.getDist(clickedPoint))
                {
                    minimum = obj.getDist(clickedPoint);
                    @object = obj;
                }
                if (@object == null)
                    @object = mapObjects[0];
            }

            if (@object!=null)
            distanceToPoints.Content = $"{Math.Round(minimum)} { @object.getTitle()}"; 

        }

        void checker()
        {
            switch (createmodecombo.SelectedIndex)
            {
                case 0:
                    {
                        if (areaspots.Count > 2)
                            addbuttoncreate.IsEnabled = true;
                        else
                            addbuttoncreate.IsEnabled = false;
                        break;
                    }
                case 4:
                    {
                        if (areaspots.Count > 1)
                            addbuttoncreate.IsEnabled = true;
                        else
                            addbuttoncreate.IsEnabled = false;
                        break;
                    }
                default :
                    {
                        addbuttoncreate.IsEnabled = true;
                        break;
                    }

            }

            if (areaspots.Count == 0)
                addbuttoncreate.IsEnabled = false;
        }

        private void Map_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            areaspots.Add(Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y));
            checker();
            clickinfoY.Content = areaspots.Last().Lng;
            clickinfoX.Content = areaspots.Last().Lat;
        }

        private void createmodecombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checker();
        }

        private void findsresult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
       
                if (SortedList.Count != 0 && findsresult.SelectedIndex != -1)
                    Map.Position = SortedList[findsresult.SelectedIndex].getFocus();
           
        }

      
        

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            findsresult.Items.Clear();
            for (int i = 0; i < mapObjects.Count; i++)
                if (mapObjects[i].objectName.Contains(whatineedtofound.Text))
                {
                    findsresult.Items.Add(mapObjects[i].objectName);
                    SortedList.Add(mapObjects[i]);
                }
        }

        private void resetpointcreate_Click(object sender, RoutedEventArgs e)
        {
            areaspots = new List<PointLatLng>();
            clickinfoX.Content = "0";
            clickinfoY.Content = "0";

        }
    }
    
}




//public ref GMapMarker findRef()
//{
//    return ref new GMapMarker(new PointLatLng());
//}

//PointLatLng point = new PointLatLng(55.016511, 82.946152);

//GMapMarker marker = new GMapMarker(point)
//{
//    Shape = new Image
//    {
//        Width = 32, // ширина маркера
//        Height = 32, // высота маркера
//        ToolTip = "Honda CR-V", // всплывающая подсказка
//        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/men.png")) // картинка
//    }
//};
//Map.Markers.Add(marker);

//GMapMarker marker = new GMapMarker(point)
//{
//    Shape = new Image
//    {
//        Width = 32, // ширина маркера
//        Height = 32, // высота маркера
//        ToolTip = "timetime", // всплывающая подсказка
//        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/notMainSpot.png")) // картинка
//    }
//};





//if (Map.Markers.Count == 0)
//{
//    Map.Markers.Add(marker);
//    clickinfoX.Content = point.Lat;
//    clickinfoY.Content = point.Lng;

//}
//else
//{

//    Map.Markers.Add(marker);     // после выхода из метода ссылка обнуляется 
//    clickinfoX.Content = point.Lat;
//    clickinfoY.Content = point.Lng;
//}
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Locomotiv.Model;
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

namespace Locomotiv.View
{
    /// <summary>
    /// Logique d'interaction pour AdminHomeView.xaml
    /// </summary>
    public partial class AdminHomeView : UserControl
    {
        public AdminHomeView()
        {
            InitializeComponent();
            InitializeMap();
        }

        private void InitializeMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            MapControl.MapProvider = GMapProviders.OpenStreetMap;

            MapControl.Position = new PointLatLng(46.8139, -71.2080);
            MapControl.Zoom = 12;
        }    
    }
}

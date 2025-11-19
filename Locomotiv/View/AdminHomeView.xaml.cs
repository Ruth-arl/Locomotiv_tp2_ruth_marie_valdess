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
using static System.Collections.Specialized.BitVector32;

namespace Locomotiv.View
{
    /// <summary>
    /// Logique d'interaction pour AdminHomeView.xaml
    /// </summary>
    public partial class AdminHomeView : UserControl
    {
        private readonly Dictionary<Station, FrameworkElement> _stationMarkers = new();
        private readonly Dictionary<Station, Border> _stationLabels = new();
        private readonly Dictionary<Voie, TextBlock> _voieLabels = new();
        private readonly List<Ellipse> _pointMarkers = new();
        private readonly Dictionary<Model.Block, Path> _blockPaths = new();

        public AdminHomeView()
        {
            InitializeComponent();
            InitializeMap();
            Loaded += AdminHomeView_Loaded;
        }

        private void AdminHomeView_Loaded(object sender, RoutedEventArgs e)
        {
            ChargerElementsCarte();
        }

        private void InitializeMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            MapControl.MapProvider = GMapProviders.OpenStreetMap;

            MapControl.Position = new PointLatLng(46.8139, -71.2080);
            MapControl.Zoom = 12;        
        }

        private void ChargerElementsCarte()
        {
            MarkerCanvas.Children.Clear();
            _stationMarkers.Clear();
            _stationLabels.Clear();
            _voieLabels.Clear();
            _pointMarkers.Clear();

            var vm = DataContext as ViewModel.AdminHomeViewModel;
            if (vm == null) return;

            MapControl.Markers.Clear();

            foreach (var s in vm.Stations)
            {
                var marker = new GMapMarker(new PointLatLng(s.Latitude, s.Longitude))
                {
                    Shape = new Ellipse
                    {
                        Width = 20,
                        Height = 20,
                        Fill = Brushes.Red,
                        Stroke = s.HasConflict ? Brushes.Black : Brushes.White,
                        StrokeThickness = 2,
                        ToolTip = $"{s.Nom} : {s.Ville}",
                        Cursor = Cursors.Hand
                    }
                };

                marker.Shape.MouseLeftButtonUp += (snd, args) =>
                {
                    vm.SelectStationCommand.Execute(s.IdStation.ToString());
                    MapControl.Position = new PointLatLng(s.Latitude, s.Longitude);
                };

                MapControl.Markers.Add(marker);

                var stationEllipse = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Transparent,
                    IsHitTestVisible = false 
                };
                MarkerCanvas.Children.Add(stationEllipse);
                _stationMarkers[s] = stationEllipse;

                var label = new TextBlock
                {
                    Text = (vm.Stations.IndexOf(s) + 1).ToString(),
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Red,
                    FontSize = 12,
                    Background = Brushes.White,
                    Padding = new Thickness(4),
                    Opacity = 0.8
                };
                var border = new Border { Child = label };
                MarkerCanvas.Children.Add(border);
                _stationLabels[s] = border;

            }

            foreach (var p in vm.PointsInteret)
            {
                var marker = new GMapMarker(new PointLatLng(p.Latitude, p.Longitude))
                {
                    Shape = new Ellipse
                    {
                        Width = 14,
                        Height = 14,
                        Fill = Brushes.Green,
                        Stroke = Brushes.White,
                        StrokeThickness = 1.5,
                        ToolTip = $"{p.Nom} : {p.Type}"
                    }
                };
                MapControl.Markers.Add(marker);
            }

            foreach (var block in vm.Blocks)
            {
                var path = new Path
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Color.FromArgb(50, 200, 0, 0)),
                    ToolTip = block.Nom,
                    IsHitTestVisible = false
                };
                ToolTipService.SetShowOnDisabled(path, true);
                MarkerCanvas.Children.Add(path);
                _blockPaths[block] = path;
            }

            UpdateAllMarkerPositions();
        }

        private void UpdateAllMarkerPositions()
        {
            foreach (var kvp in _stationMarkers)
            {
                var station = kvp.Key;
                var marker = kvp.Value;
                var point = MapControl.FromLatLngToLocal(new PointLatLng(station.Latitude, station.Longitude));
                Canvas.SetLeft(marker, point.X - marker.Width / 2);
                Canvas.SetTop(marker, point.Y - marker.Height / 2);

                if (_stationLabels.TryGetValue(station, out var label))
                {
                    Canvas.SetLeft(label, point.X + marker.Width / 2);
                    Canvas.SetTop(label, point.Y - marker.Height / 2);
                }

                foreach (var voie in station.Voies)
                {
                    if (_voieLabels.TryGetValue(voie, out var voieLabel))
                    {
                        Canvas.SetLeft(voieLabel, point.X - voieLabel.ActualWidth / 2);
                        Canvas.SetTop(voieLabel, point.Y + marker.Height / 2);
                    }
                }
            }
            foreach (var poiMarker in _pointMarkers)
            {
                if (poiMarker.Tag is not (ViewModel.AdminHomeViewModel vm, PointInteret p)) continue;
                var point = MapControl.FromLatLngToLocal(new PointLatLng(p.Latitude, p.Longitude));
                Canvas.SetLeft(poiMarker, point.X - poiMarker.Width / 2);
                Canvas.SetTop(poiMarker, point.Y - poiMarker.Height / 2);
            }

            foreach (var kvp in _blockPaths)
            {
                var block = kvp.Key;
                var path = kvp.Value;
                var localPoints = block.Coordinates.Select(p =>
                {
                    var gp = MapControl.FromLatLngToLocal(p);
                    return new Point(gp.X, gp.Y);
                }).ToList();
                var geometry = new StreamGeometry();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(localPoints[0], true, true);
                    ctx.PolyLineTo(localPoints.Skip(1).ToList(), true, true);
                }
                path.Data = geometry;
            }
        }
        private void MapControl_OnMapZoomChanged()
        {
            UpdateAllMarkerPositions();
        }


    }
}

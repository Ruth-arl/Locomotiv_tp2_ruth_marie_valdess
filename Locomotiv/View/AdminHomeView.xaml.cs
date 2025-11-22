using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Locomotiv.Model;
using Locomotiv.Model.Enums;
using Locomotiv.ViewModel;
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
        private GMapMarker? _selectedMarker;
        public AdminHomeView()
        {
            InitializeComponent();
            InitializeMap();
            this.DataContextChanged += AdminHomeView_DataContextChanged;
            this.Loaded += AdminHomeView_Loaded;
        }

        private void AdminHomeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is AdminHomeViewModel oldViewModel)
            {
                oldViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            if (DataContext is AdminHomeViewModel newViewModel)
            {
                newViewModel.PropertyChanged += ViewModel_PropertyChanged;
                LoadMarkers();
            }

        }
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AdminHomeViewModel.ItinerairesActifs) ||
                e.PropertyName == nameof(AdminHomeViewModel.Blocks))
            {
                LoadMarkers();
            }

        }
        private void AdminHomeView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMarkers();
        }

        private void InitializeMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            MapControl.MapProvider = GMapProviders.OpenStreetMap;

            MapControl.Position = new PointLatLng(46.8100, -71.2000);
            MapControl.MinZoom = 2;
            MapControl.MaxZoom = 18;
            MapControl.Zoom = 12;

            MapControl.DragButton = MouseButton.Left;
            MapControl.CanDragMap = true;
            MapControl.MouseWheelZoomEnabled = true;
        }

        private void LoadMarkers()
        {
            if (DataContext is not AdminHomeViewModel viewModel)
            {
                return;
            }

            MapControl.Markers.Clear();

            LoadBlockRoutes(viewModel);

            var pointsInteret = viewModel.PointInterets.ToList();
            foreach (var pointInteret in pointsInteret)
            {
                var marker = CreatePointInteretMarker(pointInteret);
                MapControl.Markers.Add(marker);
            }

            var stations = viewModel.Stations.ToList();
            foreach (var station in stations)
            {
                var marker = CreateStationMarker(station);
                MapControl.Markers.Add(marker);
            }

            LoadTrainMarkers(viewModel);
            LoadVoiesMarkers(viewModel);

            CenterMap(stations, pointsInteret);
        }

        private void CenterMap(IEnumerable<Station> stations, IEnumerable<PointInteret> pointsInteret)
        {
            var allPoints = stations.Select(s => new PointLatLng(s.Latitude, s.Longitude))
                            .Concat(pointsInteret.Select(p => new PointLatLng(p.Latitude, p.Longitude)))
                            .ToList();

            if (!allPoints.Any())
                return;

            double latMin = allPoints.Min(p => p.Lat);
            double latMax = allPoints.Max(p => p.Lat);
            double lngMin = allPoints.Min(p => p.Lng);
            double lngMax = allPoints.Max(p => p.Lng);

            double centerLat = (latMin + latMax) / 2.0;
            double centerLng = (lngMin + lngMax) / 2.0;
            MapControl.Position = new PointLatLng(centerLat, centerLng);

            // Ajuster le zoom pour que tous les points soient visibles
            double latDiff = latMax - latMin;
            double lngDiff = lngMax - lngMin;
            double maxDiff = Math.Max(latDiff, lngDiff);

            if (maxDiff > 0)
            {
                // Approximation logarithmique pour le zoom
                MapControl.Zoom = Math.Min(12, 12 - Math.Log(maxDiff) * 2);
            }
            else
            {
                MapControl.Zoom = 12; // Valeur par défaut si un seul point
            }
        }
        private void LoadBlockRoutes(AdminHomeViewModel viewModel)
        {
            var blocks = viewModel.Blocks?.ToList();

            if (blocks == null) return;


            foreach (var block in blocks)
            {
                var points = new List<PointLatLng>
                {
                    new PointLatLng(block.LatitudeDebut, block.LongitudeDebut),
                    new PointLatLng(block.LatitudeFin, block.LongitudeFin)
                };

                var route = new GMapRoute(points)
                {
                    Tag = block
                };

                var shape = new Path
                {
                    Stroke = GetColorForSignalStatus(block.EtatSignal),
                    StrokeThickness = 4,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    ToolTip = $"{block.Nom}\nStatut: {block.EtatSignal}\n{(block.EstOccupe ? $"Occupé par train #{block.TrainOccupantId}" : "Libre")}",
                    Opacity = 0.8
                };

                route.Shape = shape;
                route.ZIndex = 1;
                MapControl.Markers.Add(route);

                var latMilieu = (block.LatitudeDebut + block.LatitudeFin) / 2.0;
                var lonMilieu = (block.LongitudeDebut + block.LongitudeFin) / 2.0;
                var positionLabel = new PointLatLng(latMilieu, lonMilieu);

                var labelMarker = CreateBlockLabel(block, positionLabel);
                MapControl.Markers.Add(labelMarker);
            }
        }
        private void LoadTrainMarkers(AdminHomeViewModel viewModel)
        {

            var itinerairesActifs = viewModel.ItinerairesActifs?.ToList();

            if (itinerairesActifs == null || !itinerairesActifs.Any())
                return;

            foreach (var itineraire in itinerairesActifs)
            {
                try
                {
                    if (itineraire.Train == null || itineraire.BlocksTraverses == null || !itineraire.BlocksTraverses.Any())
                    {
                        continue;
                    }
                    var premierBlock = itineraire.BlocksTraverses.First();
                    var latMilieu = (premierBlock.LatitudeDebut + premierBlock.LatitudeFin) / 2.0;
                    var lonMilieu = (premierBlock.LongitudeDebut + premierBlock.LongitudeFin) / 2.0;
                    var position = new PointLatLng(latMilieu, lonMilieu);

                    var marker = CreateTrainMarker(itineraire, position);
                    MapControl.Markers.Add(marker);
                }
                catch
                {
                    // Erreur lors de l'ajout d'un train → ignorée
                }
            }
        }


        private GMapMarker CreateTrainMarker(Itineraire itineraire, PointLatLng position)
        {
            var train = itineraire.Train;

            var stationDepart = itineraire.Arrets?.OrderBy(a => a.Ordre).FirstOrDefault()?.Station?.Nom ?? "Inconnue";
            var stationArrivee = itineraire.Arrets?.OrderBy(a => a.Ordre).LastOrDefault()?.Station?.Nom ?? "Inconnue";

            var tooltipText = $"Train #{train.IdTrain} - {train.Type}\n" +
                             $"État: {train.Etat}\n" +
                             $"─────────────────────\n" +
                             $"De: {stationDepart} ({itineraire.HeureDepart:HH:mm})\n" +
                             $"Vers: {stationArrivee} ({itineraire.HeureArriveeEstimee:HH:mm})\n" +
                             $"─────────────────────\n" +
                             $"Blocks traversés: {itineraire.BlocksTraverses?.Count ?? 0}";

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                ToolTip = tooltipText,
                Cursor = Cursors.Hand
            };

            var label = new TextBlock
            {
                Text = $"Train #{train.IdTrain}",
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(220, 255, 152, 0)),
                Padding = new Thickness(4, 2, 4, 2),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            var trainIcon = new Border
            {
                Width = 24,
                Height = 24,
                Background = new SolidColorBrush(Color.FromArgb(255, 255, 152, 0)),
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(4),
                Child = new TextBlock
                {
                    Text = "🚂",
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.White
                }
            };

            stackPanel.Children.Add(trainIcon);
            stackPanel.Children.Add(label);

            var marker = new GMapMarker(position)
            {
                Shape = stackPanel,
                Offset = new Point(-12, -12),
                ZIndex = 100
            };

            return marker;
        }

        private Brush GetColorForSignalStatus(SignalState status)
        {
            return status switch
            {
                SignalState.Vert => Brushes.Green,
                SignalState.Jaune => Brushes.Orange,
                SignalState.Rouge => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        private GMapMarker CreateBlockLabel(Model.Block block, PointLatLng position)
        {
            var label = new TextBlock
            {
                Text = block.Nom,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                Padding = new Thickness(4, 2, 4, 2),
                FontSize = 8,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                ToolTip = $"{block.Nom}\nStatut: {block.EtatSignal}\n{(block.EstOccupe ? $"Occupé par train #{block.TrainOccupantId}" : "Libre")}"
            };

            var marker = new GMapMarker(position)
            {
                Shape = label,
                Tag = $"BlockLabel_{block.IdBlock}",
                Offset = new Point(-30, -10),
                ZIndex = 2
            };

            return marker;
        }

        private void LoadVoiesMarkers(AdminHomeViewModel viewModel)
        {
            var stations = viewModel.Stations.ToList();

            foreach (var station in stations)
            {
                if (station.Voies == null || !station.Voies.Any())
                    continue;

                int index = 0;
                foreach (var voie in station.Voies)
                {
                    double angle = (360.0 / station.Voies.Count) * index;
                    double angleRad = angle * Math.PI / 180.0;
                    double offset = 0.003;

                    double latVoie = station.Latitude + (offset * Math.Cos(angleRad));
                    double lonVoie = station.Longitude + (offset * Math.Sin(angleRad));

                    var positionVoie = new PointLatLng(latVoie, lonVoie);
                    var marker = CreateVoieMarker(voie, positionVoie);
                    MapControl.Markers.Add(marker);

                    index++;
                }
            }
        }

        private GMapMarker CreateVoieMarker(Voie voie, PointLatLng position)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(2)
            };

            var rectangle = new Rectangle
            {
                Width = 16,
                Height = 8,
                Fill = voie.EstOccupee ? Brushes.Orange : Brushes.LightGreen,
                Stroke = Brushes.White,
                StrokeThickness = 1,
                ToolTip = $"{voie.Nom} \nStation: {voie.Station?.Nom}\n{(voie.EstOccupee ? $"Occupée par train #{voie.IdTrainActuel}" : "Libre")}",
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var label = new TextBlock
            {
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(200, 108, 117, 125)),
                Padding = new Thickness(3, 1, 3, 1),
                FontSize = 7,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            stackPanel.Children.Add(rectangle);
            stackPanel.Children.Add(label);

            var marker = new GMapMarker(position)
            {
                Shape = stackPanel,
                Tag = $"Voie_{voie.IdVoie}",
                Offset = new Point(-8, -10),
                ZIndex = 8
            };

            return marker;
        }

        private GMapMarker CreateStationMarker(Station station)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5)
            };

            var ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red,
                Stroke = Brushes.White,
                StrokeThickness = 3,
                ToolTip = $"{station.Nom}\n{station.Ville}\nCapacité: {station.Trains.Count}/{station.CapaciteMax}",
                Cursor = Cursors.Hand,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 3)
            };

            ellipse.MouseLeftButtonDown += (s, e) => OnMarkerClick(station);

            var label = new TextBlock
            {
                Text = station.Nom,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(220, 220, 53, 69)),
                Padding = new Thickness(6, 3, 6, 3),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            stackPanel.Children.Add(ellipse);
            stackPanel.Children.Add(label);

            var marker = new GMapMarker(new PointLatLng(station.Latitude, station.Longitude))
            {
                Shape = stackPanel,
                Tag = station,
                Offset = new Point(-50, -35),
                ZIndex = 10
            };
            return marker;
        }

        private GMapMarker CreatePointInteretMarker(PointInteret pointInteret)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(3)
            };

            var ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Blue,
                Stroke = Brushes.White,
                StrokeThickness = 2,
                ToolTip = $"{pointInteret.Nom}\n{pointInteret.Type}",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 2)
            };

            var label = new TextBlock
            {
                Text = pointInteret.Nom,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(200, 13, 110, 253)),
                Padding = new Thickness(4, 2, 4, 2),
                FontSize = 8,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };

            stackPanel.Children.Add(ellipse);
            stackPanel.Children.Add(label);

            var marker = new GMapMarker(new PointLatLng(pointInteret.Latitude, pointInteret.Longitude))
            {
                Shape = stackPanel,
                Tag = pointInteret,
                Offset = new Point(-45, -28),
                ZIndex = 5
            };
            return marker;
        }

        private void OnMarkerClick(Station station)
        {
            if (DataContext is not AdminHomeViewModel viewModel)
                return;

            viewModel.SelectedStation = station;

            if (_selectedMarker != null && _selectedMarker.Shape is StackPanel oldStackPanel)
            {
                var oldEllipse = oldStackPanel.Children.OfType<Ellipse>().FirstOrDefault();
                if (oldEllipse != null)
                {
                    oldEllipse.Fill = Brushes.Red;
                }
            }

            var marker = MapControl.Markers.FirstOrDefault(m => m.Tag is Station s && s.IdStation == station.IdStation);
            if (marker != null && marker.Shape is StackPanel stackPanel)
            {
                var ellipse = stackPanel.Children.OfType<Ellipse>().FirstOrDefault();
                if (ellipse != null)
                {
                    ellipse.Fill = Brushes.Orange;
                    _selectedMarker = marker;
                }
            }
        }

    }
}

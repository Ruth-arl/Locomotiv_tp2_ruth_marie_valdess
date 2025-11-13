using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Locomotiv.ViewModel
{
    public class InteractiveMapViewModel : BaseViewModel
    {
        private readonly IMapDAL _mapDAL;
        private readonly IStationDAL _stationDAL;
        private Station? _selectedStation;

        // Collections pour la carte
        public ObservableCollection<Station> Stations { get; set; }
        public ObservableCollection<PointInteret> PointsInteret { get; set; }
        public ObservableCollection<Block> Blocks { get; set; }

        // Station sélectionnée
        public Station? SelectedStation
        {
            get => _selectedStation;
            set
            {
                _selectedStation = value;
                OnPropertyChanged();
                ChargerDetailsStation();
            }
        }

        // Détails de la station sélectionnée
        public ObservableCollection<Train> TrainsStation { get; set; }
        public ObservableCollection<Voie> VoiesStation { get; set; }

        // Commandes
        public ICommand SelectStationCommand { get; }
        public ICommand RefreshMapCommand { get; }

        // Timer pour rafraîchissement
        private DispatcherTimer _refreshTimer;

        public InteractiveMapViewModel(IMapDAL mapDAL, IStationDAL stationDAL)
        {
            _mapDAL = mapDAL;
            _stationDAL = stationDAL;

            Stations = new ObservableCollection<Station>();
            PointsInteret = new ObservableCollection<PointInteret>();
            Blocks = new ObservableCollection<Block>();
            TrainsStation = new ObservableCollection<Train>();
            VoiesStation = new ObservableCollection<Voie>();

            RefreshMapCommand = new RelayCommand(ChargerDonneesCarte);

            ChargerDonneesCarte();
            DemarrerRafraichissementAuto();
        }

        private void ChargerDonneesCarte()
        {
            // Charger toutes les données
            var stations = _mapDAL.GetAllStations();
            var points = _mapDAL.GetAllPonitInteret();
            var blocks = _mapDAL.GetAllBlocks();

            Stations.Clear();
            foreach (var s in stations) Stations.Add(s);

            PointsInteret.Clear();
            foreach (var p in points) PointsInteret.Add(p);

            Blocks.Clear();
            foreach (var b in blocks) Blocks.Add(b);
        }

        private void SelectStation(int stationId)
        {
            SelectedStation = _stationDAL.GetStationDetails(stationId);
        }

        private void ChargerDetailsStation()
        {
            TrainsStation.Clear();
            VoiesStation.Clear();

            if (SelectedStation != null)
            {
                foreach (var t in SelectedStation.Trains)
                    TrainsStation.Add(t);

                foreach (var v in SelectedStation.Voies)
                    VoiesStation.Add(v);
            }
        }

        private void DemarrerRafraichissementAuto()
        {
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(5); // Rafraîchir toutes les 5 secondes
            _refreshTimer.Tick += (s, e) => ChargerDonneesCarte();
            _refreshTimer.Start();
        }
    }
}

using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Enums;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Locomotiv.ViewModel
{
    public class StationViewModel : BaseViewModel
    {
        public const int TEXTE_NB_CARAC_MAX = 30;
        public const int TEXTE_NB_CARAC_MIN = 3;
        private readonly IUserSessionService _userSessionService;
        private readonly IStationService _stationService;
        private readonly INavigationService _navigationService;
        private readonly ITrainDAL _trainDAL;
        private readonly IStationDAL _stationDAL;

        public ObservableCollection<Station> Stations { get; set; }
        public ObservableCollection<Train> Trains { get; set; }
        public ObservableCollection<TrainStatus> Etat { get; set; }
        public ObservableCollection<TrainType> Type { get; set; }

        private Station _selectedStation;
        public Station SelectedStation
        {
            get => _selectedStation;
            set
            {
                _selectedStation = value;
                OnPropertyChanged(nameof(SelectedStation));

                if (_selectedStation != null)
                    Trains = new ObservableCollection<Train>(_selectedStation.Trains);
                else
                    Trains = new ObservableCollection<Train>();
                OnPropertyChanged(nameof(Trains));
            }
        }

        private Train _selectedTrain;
        public Train SelectedTrain
        {
            get => _selectedTrain;
            set
            {
                _selectedTrain = value;
                OnPropertyChanged(nameof(SelectedTrain));
            }
        }

        private Train _nouveauTrain;
        public Train NouveauTrain
        {
            get => _nouveauTrain;
            set
            {
                _nouveauTrain = value;
                OnPropertyChanged(nameof(NouveauTrain));
            }
        }

        private DateTime? _departDate;
    
        public DateTime? DepartDate
        {
            get => _departDate;
            set
            {
                _departDate = value;
                OnPropertyChanged(nameof(DepartDate));
                UpdateDepartDateTime();
            }
        }

        private DateTime? _arriveeDate;
    
        public DateTime? ArriveeDate
        {
            get => _arriveeDate;
            set
            {
                _arriveeDate = value;
                OnPropertyChanged(nameof(ArriveeDate));
                UpdateArriveeDateTime();
            }
        }

        private string _departTimeText = "";
  
        public string DepartTimeText
        {
            get => _departTimeText;
            set
            {
                _departTimeText = value;
                OnPropertyChanged(nameof(DepartTimeText));
                UpdateDepartDateTime();
            }
        }

        private string _arriveeTimeText = "";
    
        public string ArriveeTimeText
        {
            get => _arriveeTimeText;
            set
            {
                _arriveeTimeText = value;
                OnPropertyChanged(nameof(ArriveeTimeText));
                UpdateArriveeDateTime();
            }
        }

        private void UpdateDepartDateTime()
        {
            if (NouveauTrain == null) return;
            NouveauTrain.HeureDepart = ParseDateTime(_departDate, _departTimeText, DateTime.Now);
        }

        private void UpdateArriveeDateTime()
        {
            if (NouveauTrain == null) return;
            NouveauTrain.HeureArrivee = ParseDateTime(_arriveeDate, _arriveeTimeText, DateTime.Now.AddHours(1));
        }

  
        private DateTime ParseDateTime(DateTime? date, string timeText, DateTime defaultValue)
        {
            var selectedDate = date ?? DateTime.Today;

            if (string.IsNullOrWhiteSpace(timeText))
                return defaultValue;

            if (TryParseTime(timeText, out TimeSpan time))
                return selectedDate.Date.Add(time);

            return defaultValue;
        }

        private bool TryParseTime(string timeText, out TimeSpan time)
        {
            return TimeSpan.TryParseExact(timeText, @"HH\:mm", null, out time) ||
                   TimeSpan.TryParse(timeText, out time);
        }

        public Station SelectedStationItem
        {
            get => _selectedStation;
            set
            {
                _selectedStation = value;
                OnPropertyChanged(nameof(SelectedStationItem));
                OnPropertyChanged(nameof(Nom));
                
                LoadTrains();
            }
        }

        public string Nom
        {
            get => _selectedStation?.Nom ?? "";
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Le nom ne doit pas être vide.", nameof(Nom));

                value = value.Trim();

                if (value.Length < TEXTE_NB_CARAC_MIN || value.Length > TEXTE_NB_CARAC_MAX)
                    throw new ArgumentOutOfRangeException(nameof(Nom),
                        $"Le nom doit contenir entre {TEXTE_NB_CARAC_MIN} et {TEXTE_NB_CARAC_MAX} caractères.");

                if (_selectedStation != null) _selectedStation.Nom = value;
            }
        }

        public ICommand AddTrainCommand { get; set; }
        public ICommand DeleteTrainCommand { get; set; }
        public ICommand VoirDetailsCommand { get; set; }

        public ICollection<Voie> Voies
        {
            get => _selectedStation?.Voies ?? new List<Voie>();
        }

        /// <summary>
        /// Signaux de la station.
        /// </summary>
        public ICollection<Signal> Signaux
        {
            get => _selectedStation?.Signaux ?? new List<Signal>();
        }

        public bool IsAdmin
        {
            get
            {
                return _userSessionService.ConnectedUser != null
                       && _userSessionService.ConnectedUser.Role == UserRole.Administrateur;
            }
        }


        public StationViewModel(IStationService stationService, IUserSessionService userSessionService, IStationDAL stationDAL, INavigationService navigationService, ITrainDAL trainDAL)
        {
            _stationService = stationService;
            _userSessionService = userSessionService;
            _navigationService = navigationService;
            _stationDAL = stationDAL;
            _trainDAL = trainDAL;

            ChargerStationsSelonUtilisateur();

            NouveauTrain = new Train();

            Etat = new ObservableCollection<TrainStatus>((TrainStatus[])Enum.GetValues(typeof(TrainStatus)));
            Type = new ObservableCollection<TrainType>((TrainType[])Enum.GetValues(typeof(TrainType)));

            AddTrainCommand = new RelayCommand(Add, CanAdd);
            DeleteTrainCommand = new RelayCommand(Delete, CanDelete);
            VoirDetailsCommand = new RelayCommand(VoirDetails);

            OnPropertyChanged(nameof(IsAdmin));
            InitializeNewTrainDateTime();
        }

        private void InitializeNewTrainDateTime()
        {
            DepartDate = DateTime.Today;
            ArriveeDate = DateTime.Today;
            DepartTimeText = DateTime.Now.ToString("HH:mm");
            ArriveeTimeText = DateTime.Now.AddHours(1).ToString("HH:mm");
        }

        private void LoadTrains()
        {
            Trains.Clear();
            if (_selectedStation != null)
            {
                // Charger les trains depuis la base de données
                var trains = _trainDAL.GetTrainsByStation(_selectedStation.IdStation);
                foreach (var train in trains)
                {
                    Trains.Add(train);
                }
            }
            OnPropertyChanged(nameof(Trains));
        }
        private void ChargerStationsSelonUtilisateur()
        {
            var user = _userSessionService.ConnectedUser;
            if (user == null)
            {
                MessageBox.Show("Aucun utilisateur connecté.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Stations = new ObservableCollection<Station>();
                return;
            }
            if (user.Role == UserRole.Administrateur)
            {
                Stations = new ObservableCollection<Station>(_stationService.GetAllStations());
            }
            else
            {
                if (user.StationId.HasValue)
                {
                    var station = _stationService.GetStationById(user.StationId.Value);
                    Stations = station != null ? new ObservableCollection<Station> { station } : new ObservableCollection<Station>();
                }
                else
                {
                    Stations = new ObservableCollection<Station>();
                }
            }

            SelectedStation = Stations.FirstOrDefault();
        }

        private void Add()
        {

            if (SelectedStation.Trains.Count >= SelectedStation.CapaciteMax)
            {
                MessageBox.Show("La capacité maximale de la station est atteinte.", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NouveauTrain.IdStation = SelectedStation.IdStation;

            _trainDAL.AjouterTrain(NouveauTrain);

            Trains = new ObservableCollection<Train>(SelectedStation.Trains);
            OnPropertyChanged(nameof(Trains));

            NouveauTrain = new Train();
            OnPropertyChanged(nameof(NouveauTrain));
        }

        private bool CanAdd()
        {
            return NouveauTrain != null && IsAdmin;
        }

        private void Delete()
        {
            _trainDAL.SupprimerTrain(SelectedTrain);

            Trains = new ObservableCollection<Train>(SelectedStation.Trains);
            OnPropertyChanged(nameof(Trains));
        }

        private bool CanDelete()
        {
            return SelectedTrain != null && IsAdmin;
        }

        private void VoirDetails()
        {
            if (SelectedStation != null)
            {
                _navigationService.NavigateTo<StationDetailsViewModel>();
            }
        }

        public void SetSelectedStation(Station station)
        {
            if (station != null && Stations.Contains(station))
            {
                SelectedStationItem = station;
            }
            else if (station != null)
            {
                // Si la station n'est pas dans la liste, la recharger depuis la base de données
                var stationFromDb = _stationDAL.GetStationDetails(station.IdStation);
                if (stationFromDb != null)
                {
                    SelectedStationItem = stationFromDb;
                }
            }
        }
    }
}

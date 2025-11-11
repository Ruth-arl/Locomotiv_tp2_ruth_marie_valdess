using Locomotiv.Model;
using Locomotiv.Model.Enums;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class StationViewModel : BaseViewModel
    {
        public const int TEXTE_NB_CARAC_MAX = 30;
        public const int TEXTE_NB_CARAC_MIN = 3;

        private readonly IUserSessionService _userSessionService;

        private readonly IStationService _stationService;
        public ObservableCollection<Station> Stations { get; set; }

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

        private ObservableCollection<Train> _trains;
        public ObservableCollection<Train> Trains
        {
            get => _trains;
            set
            {
                _trains = value;
                OnPropertyChanged(nameof(Trains));
            }
        }
        public ObservableCollection<TrainStatus> Etat { get; set; }
        public ObservableCollection<TrainType> Type { get; set; }

        public ICommand AddTrainCommand { get; set; }
        public ICommand DeleteTrainCommand { get; set; }

        public bool IsAdmin => _userSessionService.ConnectedUser?.Role == UserRole.Administrateur;

        public string Nom
        {
            get => SelectedStation.Nom;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Le nom ne doit pas être vide.", nameof(Nom));

                value = value.Trim();

                if (value.Length < TEXTE_NB_CARAC_MIN || value.Length > TEXTE_NB_CARAC_MAX)
                    throw new ArgumentOutOfRangeException(nameof(Nom),
                        $"Le nom doit contenir entre {TEXTE_NB_CARAC_MIN} et {TEXTE_NB_CARAC_MAX} caractères.");

                SelectedStation.Nom = value;
            }
        }

        public string Ville
        {
            get => SelectedStation.Ville;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La ville ne doit pas être vide.", nameof(Ville));

                value = value.Trim();

                if (value.Length < TEXTE_NB_CARAC_MIN || value.Length > TEXTE_NB_CARAC_MAX)
                    throw new ArgumentOutOfRangeException(nameof(Ville),
                        $"La ville doit contenir entre {TEXTE_NB_CARAC_MIN} et {TEXTE_NB_CARAC_MAX} caractères.");

                SelectedStation.Ville = value;
            }
        }

        public int CapaciteMax
        {
            get => SelectedStation.CapaciteMax;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(CapaciteMax),
                        "La capacité maximale doit être un entier positif.");

                SelectedStation.CapaciteMax = value;
            }
        }

        public StationViewModel(IStationService stationService, IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;

            Stations = new ObservableCollection<Station>(stationService.GetAllStations());

            SelectedStation = Stations.FirstOrDefault();

            NouveauTrain = new Train();
            Etat = new ObservableCollection<TrainStatus>(Enum.GetValues(typeof(TrainStatus)).Cast<TrainStatus>());
            Type = new ObservableCollection<TrainType>(Enum.GetValues(typeof(TrainType)).Cast<TrainType>());

            AddTrainCommand = new RelayCommand(Add, CanAdd);
            DeleteTrainCommand = new RelayCommand(Delete, CanDelete);

            OnPropertyChanged(nameof(IsAdmin));
        }

        private void Add()
        {
            if (SelectedStation.Trains.Count >= SelectedStation.CapaciteMax)
            {
                MessageBox.Show("La capacité maximale de la station est atteinte.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedStation.Trains.Add(NouveauTrain);
            Trains = new ObservableCollection<Train>(SelectedStation.Trains);
            NouveauTrain = new Train();
        }

        private bool CanAdd()
        {
            return NouveauTrain != null && IsAdmin;
        }

        private void Delete()
        {
            if (SelectedTrain != null)
            {
                 SelectedStation.Trains.Remove(SelectedTrain);
        Trains = new ObservableCollection<Train>(SelectedStation.Trains);
            }
                
        }

        private bool CanDelete()
        {
            return SelectedTrain != null && IsAdmin;
        }
    }
}


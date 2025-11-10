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
        private Station station;

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

        public ObservableCollection<Train> Trains { get; set; }
        public ObservableCollection<TrainStatus> Etat { get; set; }
        public ObservableCollection<TrainType> Type { get; set; }

        public ICommand AddTrainCommand { get; set; }
        public ICommand DeleteTrainCommand { get; set; }

        public bool IsAdmin => _userSessionService.ConnectedUser?.Role == UserRole.Administrateur;

        public string Nom
        {
            get => station.Nom;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Le nom ne doit pas être vide.", nameof(Nom));

                value = value.Trim();

                if (value.Length < TEXTE_NB_CARAC_MIN || value.Length > TEXTE_NB_CARAC_MAX)
                    throw new ArgumentOutOfRangeException(nameof(Nom),
                        $"Le nom doit contenir entre {TEXTE_NB_CARAC_MIN} et {TEXTE_NB_CARAC_MAX} caractères.");

                station.Nom = value;
            }
        }

        public string Ville
        {
            get => station.Ville;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La ville ne doit pas être vide.", nameof(Ville));

                value = value.Trim();

                if (value.Length < TEXTE_NB_CARAC_MIN || value.Length > TEXTE_NB_CARAC_MAX)
                    throw new ArgumentOutOfRangeException(nameof(Ville),
                        $"La ville doit contenir entre {TEXTE_NB_CARAC_MIN} et {TEXTE_NB_CARAC_MAX} caractères.");

                station.Ville = value;
            }
        }

        public int CapaciteMax
        {
            get => station.CapaciteMax;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(CapaciteMax),
                        "La capacité maximale doit être un entier positif.");

                station.CapaciteMax = value;
            }
        }

        public StationViewModel(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;

            station = new Station();
            Trains = new ObservableCollection<Train>(station.Trains);
            NouveauTrain = new Train();
            Etat = new ObservableCollection<TrainStatus>(Enum.GetValues(typeof(TrainStatus)).Cast<TrainStatus>());
            Type = new ObservableCollection<TrainType>(Enum.GetValues(typeof(TrainType)).Cast<TrainType>());

            AddTrainCommand = new RelayCommand(Add, CanAdd);
            DeleteTrainCommand = new RelayCommand(Delete, CanDelete);
        }

        private void Add()
        {
            if (station.Trains.Count >= station.CapaciteMax)
            {
                MessageBox.Show("La capacité maximale de la station est atteinte.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            station.Trains.Add(NouveauTrain);
            NouveauTrain = new Train();
        }

        private bool CanAdd()
        {
            return NouveauTrain != null && IsAdmin;
        }

        private void Delete()
        {
            if (SelectedTrain != null)
                station.Trains.Remove(SelectedTrain);
        }

        private bool CanDelete()
        {
            return SelectedTrain != null && IsAdmin;
        }
    }
}


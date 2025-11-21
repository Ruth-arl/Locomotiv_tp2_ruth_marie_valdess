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

namespace Locomotiv.ViewModel
{
    public class StationViewModel : BaseViewModel
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IStationService _stationService;
        private readonly INavigationService _navigationService;
        private readonly ITrainDAL _trainDAL;

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

        public ICommand AddTrainCommand { get; set; }
        public ICommand DeleteTrainCommand { get; set; }
        public ICommand VoirDetailsCommand { get; set; }

        public StationViewModel(IStationService stationService, IUserSessionService userSessionService, INavigationService navigationService, ITrainDAL trainDAL)
        {
            _stationService = stationService;
            _userSessionService = userSessionService;
            _navigationService = navigationService;
            _trainDAL = trainDAL;

            ChargerStationsSelonUtilisateur();

            NouveauTrain = new Train();

            Etat = new ObservableCollection<TrainStatus>((TrainStatus[])Enum.GetValues(typeof(TrainStatus)));
            Type = new ObservableCollection<TrainType>((TrainType[])Enum.GetValues(typeof(TrainType)));

            AddTrainCommand = new RelayCommand(Add, CanAdd);
            DeleteTrainCommand = new RelayCommand(Delete, CanDelete);
            VoirDetailsCommand = new RelayCommand(VoirDetails);
        }

        private void ChargerStationsSelonUtilisateur()
        {
            var user = _userSessionService.ConnectedUser;
            if (user == null)
            {
                throw new InvalidOperationException("Aucun utilisateur connecté.");

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

        private bool IsLocomotiveValid()
        {
            if (string.IsNullOrWhiteSpace(NouveauTrain.Locomotive))
                return false;

            return NouveauTrain.Locomotive.Any(char.IsLetter);
        }


        private void Add()
        {
            if (NouveauTrain == null ||
                !IsLocomotiveValid() ||                     
                NouveauTrain.NombreWagons < 1 ||
                NouveauTrain.NombreWagons > 20)
            {
                MessageBox.Show(
                    "Veuillez remplir correctement tous les champs.\n" +
                    "Locomotive doit contenir au moins une lettre\n" +
                    "Nombre de wagons entre 1 et 20",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            return NouveauTrain != null;
        }

        private void Delete()
        {
            _trainDAL.SupprimerTrain(SelectedTrain);

            Trains = new ObservableCollection<Train>(SelectedStation.Trains);
            OnPropertyChanged(nameof(Trains));
        }

        private bool CanDelete()
        {
            return SelectedTrain != null;
        }

        private void VoirDetails()
        {
            if (SelectedStation != null)
            {
                _navigationService.NavigateTo<StationDetailsViewModel>();
            }
        }
    }
}

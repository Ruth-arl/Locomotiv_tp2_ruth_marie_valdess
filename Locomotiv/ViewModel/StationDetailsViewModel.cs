using Locomotiv.Model;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class StationDetailsViewModel : BaseViewModel
    {
        private readonly IStationService _stationService;
        private readonly IUserSessionService _userSessionService;
        private readonly INavigationService _navigationService;

        private Station _station;
        public Station Station
        {
            get => _station;
            set
            {
                _station = value;
                OnPropertyChanged();
                Trains = new ObservableCollection<Train>(_station?.Trains ?? []);
                Voies = new ObservableCollection<Voie>(_station?.Voies ?? []);
                Signaux = new ObservableCollection<Signal>(_station?.Signaux ?? []);
                TrainsEnGare = new ObservableCollection<Train>(_station?.Trains?.Where(t => t.EstEnGare) ?? []);
            }
        }

        public ObservableCollection<Train> Trains { get; private set; }
        public ObservableCollection<Voie> Voies { get; private set; }
        public ObservableCollection<Signal> Signaux { get; private set; }
        public ObservableCollection<Train> TrainsEnGare { get; private set; }

        public ICommand RetourCommand { get; }

        public StationDetailsViewModel(IStationService stationService, IUserSessionService userSessionService, INavigationService navigationService)
        {
            _stationService = stationService;
            _userSessionService = userSessionService;
            _navigationService = navigationService;

            ChargerStationAssignée();
            RetourCommand = new RelayCommand(Retour);
        }

        private void ChargerStationAssignée()
        {
            var user = _userSessionService.ConnectedUser;
            if (user?.StationId != null)
            {
                Station = _stationService.GetStationById(user.StationId.Value);
            }
        }

        private void Retour()
        {
            _navigationService.NavigateTo<StationViewModel>();
        }
    }
}

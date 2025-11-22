using Locomotiv.Model;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services;
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
        private readonly IMessageService _messageService;

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


        public StationDetailsViewModel(
            IStationService stationService,
            IUserSessionService userSessionService,
            INavigationService navigationService,
            IMessageService messageService)
        {
            _stationService = stationService;
            _userSessionService = userSessionService;
            _navigationService = navigationService;
            _messageService = messageService;

            var user = _userSessionService.ConnectedUser;

            if (user == null)
            {
                _messageService.ShowError("Aucun utilisateur n'est connecté.");
                return;
            }

            if (!user.StationId.HasValue)
            {
                _messageService.Show("Vous n'êtes assigné(e) à aucune station.");
                return;
            }

            Station = _stationService.GetStationById(user.StationId.Value);

            if (Station == null)
            {
                _messageService.ShowError("Impossible de charger la station.");
            }
        }

    }
}

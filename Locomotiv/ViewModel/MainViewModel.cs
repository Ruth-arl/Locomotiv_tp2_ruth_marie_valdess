using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;

namespace Locomotiv.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;

        public INavigationService NavigationService
        {
            get => _navigationService;
        }

        public ICommand NavigateToStationViewCommand { get; set; }

        public IUserSessionService UserSessionService
        {
            get => _userSessionService;
        }

        public ICommand NavigateToConnectUserViewCommand { get; set; }
        public ICommand NavigateToHomeViewCommand { get; set; }

        public ICommand DisconnectCommand { get; }

        public ICommand NavigateToStationDetailsCommand { get; }


        private void Disconnect()
        {
            _userSessionService.ConnectedUser = null;
            OnPropertyChanged(nameof(UserSessionService.IsUserConnected));
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }

        public MainViewModel(INavigationService navigationService, IUserSessionService userSessionService)
        {
            _navigationService = navigationService;
            _userSessionService = userSessionService;

            NavigateToConnectUserViewCommand = new RelayCommand(() => NavigationService.NavigateTo<ConnectUserViewModel>());
            NavigateToHomeViewCommand = new RelayCommand(() => NavigationService.NavigateTo<HomeViewModel>());
            DisconnectCommand = new RelayCommand(Disconnect, () => UserSessionService.IsUserConnected);

            NavigateToStationViewCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo<StationViewModel>();
            });

            NavigationService.NavigateTo<HomeViewModel>();

            NavigateToStationDetailsCommand = new RelayCommand(() =>
            {
                if (UserSessionService.ConnectedUser != null)
                    NavigationService.NavigateTo<StationDetailsViewModel>();
            });
        }
    }
}

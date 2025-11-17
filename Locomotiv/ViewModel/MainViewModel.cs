using Locomotiv.Model;
using Locomotiv.Model.Enums;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

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

        private void NavigateToHome()
        {
            if (_userSessionService.ConnectedUser?.Role == UserRole.Administrateur)
            {
                _navigationService.NavigateTo<AdminHomeViewModel>();
            }
            else
            {
                _navigationService.NavigateTo<HomeViewModel>();
            }
        }
        public MainViewModel(INavigationService navigationService, IUserSessionService userSessionService)
        {
            _navigationService = navigationService;
            _userSessionService = userSessionService;

            NavigateToConnectUserViewCommand = new RelayCommand(() => NavigationService.NavigateTo<ConnectUserViewModel>());
            NavigateToHomeViewCommand = new RelayCommand(NavigateToHome);
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

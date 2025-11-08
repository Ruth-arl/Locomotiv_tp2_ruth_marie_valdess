using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Locomotiv.Model;
using Locomotiv.View;

namespace Locomotiv.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IUserDAL _userDAL;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;

        public User? ConnectedUser
        {
            get => _userSessionService.ConnectedUser;
        }

        public string WelcomeMessage
        {
            get
            {
                if (ConnectedUser == null)
                    return "Bienvenue chère personne inconnue!";

                string stationInfo = ConnectedUser.Station != null
                    ? $"Station : {ConnectedUser.Station.Nom} ({ConnectedUser.Station.Ville})"
                    : "Aucune station assignée.";

                return $"Bienvenue {ConnectedUser.Prenom}!\n{stationInfo}";
            }
        }


        public HomeViewModel(IUserDAL userDAL, INavigationService navigationService, IUserSessionService userSessionService)
        {
            _userDAL = userDAL;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            LogoutCommand = new RelayCommand(Logout, CanLogout);
        }

        // Commande pour la déconnexion
        public ICommand LogoutCommand { get; set; }

        // Méthode pour gérer la déconnexion de l'utilisateur
        private void Logout()
        {
            _userSessionService.Disconnect();
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }

        // Vérifie si la commande de déconnexion peut être exécutée
        private bool CanLogout()
        {
            return _userSessionService.IsUserConnected;
        }
    }
}

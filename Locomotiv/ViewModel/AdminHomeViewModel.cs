using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{  
    public class AdminHomeViewModel : BaseViewModel
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
            get => ConnectedUser == null ? "Bienvenue chère Adminstrateur" : $"Bienvenue {ConnectedUser.Prenom}!";
        }

        public AdminHomeViewModel(IUserDAL userDAL, INavigationService navigationService, IUserSessionService userSessionService)
        {
            _userDAL = userDAL;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            LogoutCommand = new RelayCommand(Logout, CanLogout);

           
        }

        private void OnStationSelected(int idStation)
        {
            // Tu peux ouvrir une fenêtre, changer un panneau, etc.
            System.Diagnostics.Debug.WriteLine($"Station sélectionnée : {idStation}");
        }

       

        // Commande pour la déconnexion
        public ICommand LogoutCommand { get; set; }

        // Méthode pour gérer la déconnexion de l'utilisateur
        private void Logout()
        {
            _userSessionService.ConnectedUser = null;
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }

        // Vérifie si la commande de déconnexion peut être exécutée sss
        private bool CanLogout()
        {
            return _userSessionService.IsUserConnected;
        }
    }
       
   
}

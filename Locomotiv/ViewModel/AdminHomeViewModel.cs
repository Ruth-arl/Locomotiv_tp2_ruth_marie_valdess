using GMap.NET;
using Locomotiv.Model;
using Locomotiv.Model.Enums;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
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

        public ObservableCollection<Station> Stations { get; set; }
        public ObservableCollection<PointInteret> PointsInteret { get; set; }
        public ObservableCollection<Block> Blocks { get; set; }
        public ObservableCollection<Train> Trains { get; set; } = new ObservableCollection<Train>();

        public ICommand SelectStationCommand { get; }

        // Commande pour la déconnexion
        public ICommand LogoutCommand { get; set; }

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

            Stations = new ObservableCollection<Station>();
            PointsInteret = new ObservableCollection<PointInteret>();
            Blocks = new ObservableCollection<Block>();
            SelectStationCommand = new RelayCommand(() => OnStationSelected("1"));

            ChargerStations();
            ChargerPointsInteret();
            ChargerBlocks();
        }

        private void ChargerStations()
        {
            Stations.Add(new Station
            {
                IdStation = 1,
                Nom = "Gare Québec Gatineau",
                Ville = "Quebec",
                Latitude = 46.8139,
                Longitude = -71.2080,
                CapaciteMax = 100
            });

            Stations.Add(new Station
            {
                IdStation = 2,
                Nom = "Gare du Palais",
                Ville = "Nord Quebec",
                Latitude = 46.8150,
                Longitude = -71.2050,
                CapaciteMax = 100
            });

            Stations.Add(new Station
            {
                IdStation = 2,
                Nom = "Gare CN",
                Ville = "Québec",
                Latitude = 46.8170,
                Longitude = -71.2100,
                CapaciteMax = 100
            });
        }

        private void ChargerPointsInteret()
        {
            PointsInteret.Add(new PointInteret
            {
                Nom = "Vers Charlevoix",
                Type = "Destination",
                Latitude = 46.8200,
                Longitude = -71.2150
            });

            PointsInteret.Add(new PointInteret
            {
                Nom = "Baie de Beauport",
                Type = "Destination",
                Latitude = 46.8250,
                Longitude = -71.2000
            });

            PointsInteret.Add(new PointInteret
            {
                Nom = "Port de Québec",
                Type = "Destination",
                Latitude = 46.8100,
                Longitude = -71.1900
            });

            PointsInteret.Add(new PointInteret
            {
                Nom = "Centre de distribution",
                Type = "Logistique",
                Latitude = 46.8050,
                Longitude = -71.2200
            });

            PointsInteret.Add(new PointInteret
            {
                Nom = "Vers la rive-sud",
                Type = "Destination",
                Latitude = 46.8000,
                Longitude = -71.2300
            });

            PointsInteret.Add(new PointInteret
            {
                Nom = "Vers le nord",
                Type = "Destination",
                Latitude = 46.8350,
                Longitude = -71.2100
            });
        }

        private void ChargerBlocks()
        {
            Blocks.Add(new Block
            {
                Nom = "Zone A",
                StationId = 1,
                Coordinates = new List<PointLatLng>
                {
                    new PointLatLng(46.8130, -71.2080),
                    new PointLatLng(46.8140, -71.2090),
                    new PointLatLng(46.8150, -71.2070),
                }
            });

            Blocks.Add(new Block
            {
                Nom = "Zone B",
                StationId = 2,
                Coordinates = new List<PointLatLng>
                {
                    new PointLatLng(46.8150, -71.2050),
                    new PointLatLng(46.8160, -71.2060),
                    new PointLatLng(46.8170, -71.2040),
                }
            });

            Blocks.Add(new Block
            {
                Nom = "Zone C",
                StationId = 3,
                Coordinates = new List<PointLatLng>
                {
                    new PointLatLng(46.8170, -71.2100),
                    new PointLatLng(46.8180, -71.2110),
                    new PointLatLng(46.8190, -71.2090),
                }
            });
        }

       

        private void OnStationSelected(string idStation)
        {
            if (int.TryParse(idStation, out int stationId))
            {
                var station = Stations.FirstOrDefault(s => s.IdStation == stationId);
                if (station != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Station sélectionnée : {station.Nom}");
                }
            }
        }

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

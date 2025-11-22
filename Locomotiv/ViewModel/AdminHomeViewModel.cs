using GMap.NET;
using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Enums;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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
        private readonly IStationDAL _stationDAL;
        private readonly ITrainDAL _trainDAL;

        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;
        private readonly ApplicationDbContext _context;

        public ICommand LogoutCommand { get; set; }
        public ICommand ManageStationCommand { get; set; }
        public ICommand PlanItineraireCommand { get; set; }
        public ICommand AnnulerItineraireCommand { get; set; }
        public ICommand TerminerItineraireCommand { get; set; }
        public ICommand ToggleSidebarCommand { get; set; }
        public ICommand SelectStationCommand { get; }


        private bool _isSidebarVisible = true;
        public bool IsSidebarVisible
        {
            get => _isSidebarVisible;
            set
            {
                _isSidebarVisible = value;
                OnPropertyChanged(nameof(IsSidebarVisible));
            }
        }

        private Train? _selectedTrain;
        public Train? SelectedTrain
        {
            get => _selectedTrain;
            set
            {
                _selectedTrain = value;
                OnPropertyChanged(nameof(SelectedTrain));
            }
        }

        private Station? _departureStation;
        public Station? DepartureStation
        {
            get => _departureStation;
            set
            {
                _departureStation = value;
                OnPropertyChanged(nameof(DepartureStation));
            }
        }

        private Station? _arrivalStation;
        public Station? ArrivalStation
        {
            get => _arrivalStation;
            set
            {
                _arrivalStation = value;
                OnPropertyChanged(nameof(ArrivalStation));
            }
        }

        private Itineraire? _selectedItineraire;
        public Itineraire? SelectedItineraire
        {
            get => _selectedItineraire;
            set
            {
                _selectedItineraire = value;
                OnPropertyChanged(nameof(SelectedItineraire));
            }
        }

        public IEnumerable<Itineraire> ItinerairesActifs
        {
            get
            {
                try
                {
                    return _context?.Itineraires?
                        .Include(i => i.Train)
                        .Include(i => i.BlocksTraverses)
                        .Include(i => i.Arrets)
                            .ThenInclude(a => a.Station)
                        .Where(i => i.Train != null && i.Train.Etat == TrainStatus.EnTransit)
                        .ToList() ?? new List<Itineraire>();
                }
                catch (Exception ex)
                {
                    return new List<Itineraire>();
                }
            }
        }

        public IEnumerable<Train> AvailableTrains
        {
            get
            {
                try
                {
                    return _trainDAL?.GetAll()?.Where(t => t.Etat == TrainStatus.EnGare) ?? new List<Train>();
                }
                catch
                {
                    return new List<Train>();
                }
            }
        }

        public AdminHomeViewModel(IUserDAL userDAL, IStationDAL stationDAL, ITrainDAL trainDAL, INavigationService navigationService, IUserSessionService userSessionService, ApplicationDbContext context)
        {
            _userDAL = userDAL;
            _stationDAL = stationDAL;
            _trainDAL = trainDAL;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            _context = context;


            LogoutCommand = new RelayCommand(Logout, CanLogout);
            ManageStationCommand = new RelayCommand(ManageStation, CanManageStation);
            PlanItineraireCommand = new RelayCommand(PlanItineraire, CanPlanItineraire);
            AnnulerItineraireCommand = new RelayCommand(AnnulerItineraire, CanAnnulerItineraire);
            TerminerItineraireCommand = new RelayCommand(TerminerItineraire, CanTerminerItineraire);
            ToggleSidebarCommand = new RelayCommand(ToggleSidebar, CanToggleSidebar);
        }

        private Station? _selectedStation;
        public Station? SelectedStation
        {
            get => _selectedStation;
            set
            {
                _selectedStation = value;
                OnPropertyChanged(nameof(SelectedStation));
                OnPropertyChanged(nameof(IsStationSelected));

                if (value != null && !IsSidebarVisible)
                {
                    IsSidebarVisible = true;
                }
            }
        }

        public bool IsStationSelected => SelectedStation != null;

        public IEnumerable<Station> Stations
        {
            get
            {
                try
                {
                    return _stationDAL?.GetAll() ?? new List<Station>();
                }
                catch (Exception ex)
                {
                    return new List<Station>();
                }
            }
        }

        public IEnumerable<PointInteret> PointInterets
        {
            get
            {
                try
                {
                    return _context?.PointsInteret?.ToList() ?? new List<PointInteret>();
                }
                catch
                {
                    return new List<PointInteret>();
                }
            }
        }

        public IEnumerable<Block> Blocks
        {
            get
            {
                try
                {
                    return _context?.Blocks?.ToList() ?? new List<Block>();
                }
                catch
                {
                    return new List<Block>();
                }
            }
        }

        private bool CanManageStation()
        {
            return SelectedStation != null;
        }


        private void PlanItineraire()
        {
            try
            {
                if (!ValiderParametresPlanification())
                    return;

                var blocksItineraire = CalculerBlocksItineraire(DepartureStation, ArrivalStation);

                if (!ValiderBlocksCalcules(blocksItineraire))
                    return;

                if (!ValiderReglesSecurite(blocksItineraire))
                {
                    AfficherErreurSecurite();
                    return;
                }

                var itineraire = CreerItineraire(blocksItineraire);
                SauvegarderItineraire(itineraire);
                MettreAJourEtatTrain(SelectedTrain);
                OccuperBlocks(blocksItineraire, SelectedTrain.IdTrain);

                AfficherSuccesPlanification(blocksItineraire.Count());
                RefreshView();
            }
            catch (Exception ex)
            {
                GererErreurPlanification(ex);
            }
        }

        private void ManageStation()
        {
            if (SelectedStation == null) return;

            _navigationService.NavigateTo<StationViewModel>();

            if (_navigationService.CurrentView is StationViewModel stationVM)
            {
                stationVM.SetSelectedStation(SelectedStation);
            }
        }

        private bool ValiderParametresPlanification()
        {
            if (SelectedTrain == null || DepartureStation == null || ArrivalStation == null)
            {
                return false;
            }
            return true;
        }


        private bool ValiderBlocksCalcules(IEnumerable<Block> blocks)
        {
            if (blocks == null || !blocks.Any())
            {
                System.Windows.MessageBox.Show(
                    "Impossible de calculer un itinéraire entre ces stations.",
                    "Erreur",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                return false;
            }
            return true;
        }


        private void AfficherErreurSecurite()
        {
            System.Windows.MessageBox.Show(
                "L'itinéraire ne respecte pas les règles de sécurité (blocks occupés ou conflits).",
                "Erreur de sécurité",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
        }


        private Itineraire CreerItineraire(IEnumerable<Block> blocksItineraire)
        {
            var heureDepart = DateTime.Now;
            var heureArrivee = DateTime.Now.AddHours(1);

            return new Itineraire
            {
                IdTrain = SelectedTrain.IdTrain,
                Train = SelectedTrain,
                HeureDepart = heureDepart,
                HeureArriveeEstimee = heureArrivee,
                BlocksTraverses = blocksItineraire.ToList(),
                Arrets = CreerArrets(heureDepart, heureArrivee)
            };
        }


        private List<ArretItineraire> CreerArrets(DateTime heureDepart, DateTime heureArrivee)
        {
            return new List<ArretItineraire>
            {
                CreerArret(0, DepartureStation, heureDepart, heureDepart),
                CreerArret(1, ArrivalStation, heureArrivee, heureArrivee)
            };
        }


        private ArretItineraire CreerArret(int ordre, Station station, DateTime heureArrivee, DateTime heureDepart)
        {
            return new ArretItineraire
            {
                Ordre = ordre,
                IdStation = station.IdStation,
                Station = station,
                HeureArrivee = heureArrivee,
                HeureDepart = heureDepart,
                DureeArret = 0
            };
        }

        private void SauvegarderItineraire(Itineraire itineraire)
        {
            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();
        }

        private void MettreAJourEtatTrain(Train train)
        {
            train.Etat = TrainStatus.EnTransit;
            _context.Trains.Update(train);
            _context.SaveChanges();
        }


        private void AfficherSuccesPlanification(int nombreBlocks)
        {
            System.Windows.MessageBox.Show(
                $"Itinéraire planifié avec succès!\n{nombreBlocks} blocks traversés.",
                "Succès",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        private void GererErreurPlanification(Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Erreur lors de la planification de l'itinéraire:\n{ex.Message}",
                "Erreur",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }


        private bool CanPlanItineraire()
        {
            return SelectedTrain != null && DepartureStation != null && ArrivalStation != null;
        }


        private IEnumerable<Block> CalculerBlocksItineraire(Station depart, Station arrivee)
        {
            var allBlocks = _context.Blocks.ToList();

            var blocksLibres = allBlocks.Where(b => b.TrainOccupantId == null).ToList();

            var graphe = ConstruireGrapheBlocks(blocksLibres);

            var blocksDepart = TrouverBlocksProches(depart.Latitude, depart.Longitude, blocksLibres, 0.02);
            var blocksArrivee = TrouverBlocksProches(arrivee.Latitude, arrivee.Longitude, blocksLibres, 0.02);

            if (!blocksDepart.Any() || !blocksArrivee.Any())
                return new List<Block>();

            List<Block> meilleurChemin = null;
            int meilleureLongueur = int.MaxValue;

            foreach (var blockDepart in blocksDepart)
            {
                foreach (var blockArrivee in blocksArrivee)
                {
                    var chemin = TrouverCheminBFS(blockDepart, blockArrivee, graphe);
                    if (chemin != null && chemin.Count < meilleureLongueur)
                    {
                        meilleurChemin = chemin;
                        meilleureLongueur = chemin.Count;
                    }
                }
            }

            return meilleurChemin ?? new List<Block>();
        }

        private Dictionary<int, List<Block>> ConstruireGrapheBlocks(List<Block> blocks)
        {
            var graphe = new Dictionary<int, List<Block>>();

            foreach (var block in blocks)
            {
                graphe[block.IdBlock] = new List<Block>();

                foreach (var autreBlock in blocks)
                {
                    if (block.IdBlock == autreBlock.IdBlock)
                        continue;

                    double distanceDebut = CalculerDistance(
                        block.LatitudeFin, block.LongitudeFin,
                        autreBlock.LatitudeDebut, autreBlock.LongitudeDebut);

                    double distanceFin = CalculerDistance(
                        block.LatitudeFin, block.LongitudeFin,
                        autreBlock.LatitudeFin, autreBlock.LongitudeFin);

                    if (distanceDebut < 0.005 || distanceFin < 0.005)
                    {
                        graphe[block.IdBlock].Add(autreBlock);
                    }
                }
            }

            return graphe;
        }

        private List<Block> TrouverBlocksProches(double lat, double lon, List<Block> blocks, double seuil)
        {
            return blocks.Where(b =>
                CalculerDistance(lat, lon, b.LatitudeDebut, b.LongitudeDebut) < seuil ||
                CalculerDistance(lat, lon, b.LatitudeFin, b.LongitudeFin) < seuil
            ).ToList();
        }

        private List<Block> TrouverCheminBFS(Block depart, Block arrivee, Dictionary<int, List<Block>> graphe)
        {
            var queue = new Queue<List<Block>>();
            var visited = new HashSet<int>();

            queue.Enqueue(new List<Block> { depart });
            visited.Add(depart.IdBlock);

            while (queue.Count > 0)
            {
                var chemin = queue.Dequeue();
                var dernierBlock = chemin[chemin.Count - 1];

                if (dernierBlock.IdBlock == arrivee.IdBlock)
                    return chemin;

                if (!graphe.ContainsKey(dernierBlock.IdBlock))
                    continue;

                foreach (var voisin in graphe[dernierBlock.IdBlock])
                {
                    if (!visited.Contains(voisin.IdBlock))
                    {
                        visited.Add(voisin.IdBlock);
                        var nouveauChemin = new List<Block>(chemin) { voisin };
                        queue.Enqueue(nouveauChemin);
                    }
                }
            }

            return null;
        }


        private double CalculerDistance(double lat1, double lon1, double lat2, double lon2)
        {
            return Math.Sqrt(Math.Pow(lat2 - lat1, 2) + Math.Pow(lon2 - lon1, 2));
        }

        private bool ValiderReglesSecurite(IEnumerable<Block> blocks)
        {
            var blocksOccupes = blocks.Where(b => b.EstOccupe).ToList();

            if (blocksOccupes.Any())
            {
                return false;
            }

            var allBlocks = _context.Blocks.ToList();
            var blocksOccupesParTrains = allBlocks.Where(b => b.TrainOccupantId != null).ToList();

            if (!blocksOccupesParTrains.Any())
                return true;

            var graphe = ConstruireGrapheBlocks(allBlocks);

            foreach (var blockItineraire in blocks)
            {
                foreach (var blockOccupe in blocksOccupesParTrains)
                {
                    if (blockItineraire.IdBlock == blockOccupe.IdBlock)
                        return false;

                    if (graphe.ContainsKey(blockOccupe.IdBlock))
                    {
                        var blocksAdjacents = graphe[blockOccupe.IdBlock];
                        if (blocksAdjacents.Any(b => b.IdBlock == blockItineraire.IdBlock))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void OccuperBlocks(IEnumerable<Block> blocks, int trainId)
        {
            var blockIds = blocks.Select(b => b.IdBlock).ToList();

            var blocksAOccuper = _context.Blocks.Where(b => blockIds.Contains(b.IdBlock)).ToList();

            foreach (var block in blocksAOccuper)
            {
                block.TrainOccupantId = trainId;
                block.EtatSignal = SignalState.Rouge;
                _context.Blocks.Update(block);
            }
            _context.SaveChanges();
        }

        private void AnnulerItineraire()
        {
            if (SelectedItineraire == null) return;

            CompleterItineraire(
                "Itinéraire annulé avec succès. Le train est remis en gare et les blocks sont libérés.",
                "Erreur lors de l'annulation de l'itinéraire");
        }

        private bool CanAnnulerItineraire()
        {
            return SelectedItineraire != null;
        }

        private void TerminerItineraire()
        {
            if (SelectedItineraire == null) return;

            CompleterItineraire(
                "Itinéraire terminé avec succès. Le train est arrivé à destination et les blocks sont libérés.",
                "Erreur lors de la finalisation de l'itinéraire");
        }

        private bool CanTerminerItineraire()
        {
            return SelectedItineraire != null;
        }

        private void ToggleSidebar()
        {
            IsSidebarVisible = !IsSidebarVisible;
        }

        private bool CanToggleSidebar()
        {
            return true;
        }

        private void CompleterItineraire(string messageSucces, string messageTitreErreur)
        {
            try
            {
                LibererBlocks(SelectedItineraire.IdTrain);
                RemettreTrainEnGare(SelectedItineraire.IdTrain);
                SupprimerItineraire(SelectedItineraire);

                AfficherMessageSucces(messageSucces);
                RefreshView();
            }
            catch (Exception ex)
            {
                AfficherMessageErreur($"{messageTitreErreur}: {ex.Message}", "Erreur");
            }
        }

        private void RemettreTrainEnGare(int trainId)
        {
            var train = _context.Trains.Find(trainId);
            if (train != null)
            {
                train.Etat = TrainStatus.EnGare;
                _context.Trains.Update(train);
            }
        }

        private void SupprimerItineraire(Itineraire itineraire)
        {
            _context.Itineraires.Remove(itineraire);
            _context.SaveChanges();
        }

        private void AfficherMessageSucces(string message)
        {
            System.Windows.MessageBox.Show(message, "Succès",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        private void AfficherMessageErreur(string message, string titre)
        {
            System.Windows.MessageBox.Show(message, titre,
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }

        private void LibererBlocks(int trainId)
        {
            var blocksOccupes = _context.Blocks.Where(b => b.TrainOccupantId == trainId).ToList();
            foreach (var block in blocksOccupes)
            {
                block.TrainOccupantId = null;
                block.EtatSignal = SignalState.Vert;
                _context.Blocks.Update(block);
            }
            _context.SaveChanges();
        }

        private void RefreshView()
        {
            OnPropertyChanged(nameof(ItinerairesActifs));
            OnPropertyChanged(nameof(AvailableTrains));
            OnPropertyChanged(nameof(Blocks));
            SelectedItineraire = null;
        }

        private void Logout()
        {
            _userSessionService.Disconnect();
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }


        private bool CanLogout()
        {
            return _userSessionService.IsUserConnected;
        }

    }


}

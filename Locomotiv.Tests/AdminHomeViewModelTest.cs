using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Enums;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.ViewModel;
using Moq;
using Xunit;
using static System.Collections.Specialized.BitVector32;

namespace Locomotiv.Tests
{
    public class AdminHomeViewModelTest
    {
        private readonly Mock<IUserDAL> _mockUserDAL;
        private readonly Mock<IStationDAL> _mockStationDAL;
        private readonly Mock<ITrainDAL> _mockTrainDAL;
        private readonly Mock<IUserSessionService> _mockUserSession;
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<ApplicationDbContext> _mockContext;

        public AdminHomeViewModelTest()
        {
            _mockUserDAL = new Mock<IUserDAL>();
            _mockStationDAL = new Mock<IStationDAL>();
            _mockTrainDAL = new Mock<ITrainDAL>();
            _mockUserSession = new Mock<IUserSessionService>();
            _mockNavigation = new Mock<INavigationService>();
            _mockContext = new Mock<ApplicationDbContext>();
        }

        [Fact]
        public void Constructeur_DoitInitialiserLaListeDesStations()
        {
            // Arrange
            var stations = new List<Station>
            {
                new Station { IdStation = 1, Nom = "Gare 1" },
                new Station { IdStation = 2, Nom = "Gare 2" }
            };
            _mockStationDAL.Setup(s => s.GetAll()).Returns(stations);

            // Act
            var vm = new AdminHomeViewModel(_mockUserDAL.Object, _mockStationDAL.Object, _mockTrainDAL.Object,
                _mockNavigation.Object, _mockUserSession.Object, _mockContext.Object);

            // Assert
            Assert.NotNull(vm.Stations);
            Assert.Equal(2, vm.Stations.Count());
        }

        [Fact]
        public void SelectionStation_DoitMettreAJourSelectedStation()
        {
            // Arrange
            var station = new Station { IdStation = 1, Nom = "Gare Test" };
            var stations = new List<Station> { station };
            _mockStationDAL.Setup(s => s.GetAll()).Returns(stations);

            var vm = new AdminHomeViewModel(_mockUserDAL.Object, _mockStationDAL.Object, _mockTrainDAL.Object,
                _mockNavigation.Object, _mockUserSession.Object, _mockContext.Object);

            // Act
            vm.SelectedStation = station;

            // Assert
            Assert.Equal(station, vm.SelectedStation);
            Assert.True(vm.IsStationSelected);
        }

        [Fact]
        public void IsSidebarVisible_DoitEtreTrueParDefaut()
        {
            // Arrange & Act
            var vm = new AdminHomeViewModel(_mockUserDAL.Object, _mockStationDAL.Object, _mockTrainDAL.Object,
                _mockNavigation.Object, _mockUserSession.Object, _mockContext.Object);

            // Assert
            Assert.True(vm.IsSidebarVisible);
        }

        [Fact]
        public void ToggleSidebar_DoitInverserLaVisibilite()
        {
            // Arrange
            var vm = new AdminHomeViewModel(_mockUserDAL.Object, _mockStationDAL.Object, _mockTrainDAL.Object,
                _mockNavigation.Object, _mockUserSession.Object, _mockContext.Object);
            var initialState = vm.IsSidebarVisible;

            // Act
            vm.ToggleSidebarCommand.Execute(null);

            // Assert
            Assert.NotEqual(initialState, vm.IsSidebarVisible);
        }

        [Fact]
        public void ManageStationCommand_AvecStationSelectionnee_DoitNaviguerVersStationView()
        {
            // Arrange
            var station = new Station { IdStation = 1, Nom = "Gare Test" };
            var stations = new List<Station> { station };
            _mockStationDAL.Setup(s => s.GetAll()).Returns(stations);

            var vm = new AdminHomeViewModel(_mockUserDAL.Object, _mockStationDAL.Object, _mockTrainDAL.Object,
                _mockNavigation.Object, _mockUserSession.Object, _mockContext.Object);
            vm.SelectedStation = station;

            // Act
            vm.ManageStationCommand.Execute(null);

            // Assert
            _mockNavigation.Verify(n => n.NavigateTo<StationViewModel>(), Times.Once);
        }

        [Fact]
        public void AvailableTrains_DoitRetournerSeulementLesTrainsEnGare()
        {
            // Arrange
            var trains = new List<Train>
            {
                new Train { IdTrain = 1, Etat = TrainStatus.EnGare },
                new Train { IdTrain = 2, Etat = TrainStatus.EnTransit }
            };

            _mockTrainDAL.Setup(t => t.GetAll()).Returns(trains);

            // Act
            var vm = new AdminHomeViewModel(_mockUserDAL.Object, _mockStationDAL.Object, _mockTrainDAL.Object,
                _mockNavigation.Object, _mockUserSession.Object, _mockContext.Object);

            // Assert
            Assert.Single(vm.AvailableTrains);
            Assert.Equal(TrainStatus.EnGare, vm.AvailableTrains.First().Etat);
        }
        private AdminHomeViewModel CreateVM(
        Mock<IUserDAL>? userDalMock = null,
        Mock<INavigationService>? navMock = null,
        Mock<IUserSessionService>? sessionMock = null,
        Mock<IStationService>? stationMock = null)
        {
            var dal = userDalMock ?? new Mock<IUserDAL>();
            var nav = navMock ?? new Mock<INavigationService>();
            var session = sessionMock ?? new Mock<IUserSessionService>();
            var station = stationMock ?? new Mock<IStationService>();
            var messageMock = new Mock<IMessageService>();


            return new AdminHomeViewModel(
                dal.Object,
                nav.Object,
                session.Object,
                station.Object,
                messageMock.Object
            );
        }

        [Fact]
        public void Constructeur_DoitInitialiserStations()
        {
            var vm = CreateVM();
            Assert.NotEmpty(vm.Stations);
        }

        [Fact]
        public void OpenDetailsStation_AdminSansStation_NeDoitPasNaviguer()
        {
            var userDalMock = new Mock<IUserDAL>();
            var navMock = new Mock<INavigationService>();
            var sessionMock = new Mock<IUserSessionService>();
            var stationMock = new Mock<IStationService>();

            sessionMock.Setup(s => s.ConnectedUser)
                       .Returns(new User { Id = 1, Role = UserRole.Administrateur });

            userDalMock.Setup(d => d.GetUserWithStation(1))
                       .Returns(new User { Id = 1, Station = null });

            var vm = CreateVM(userDalMock, navMock, sessionMock, stationMock);

            vm.DetailsStationCommand.Execute(null);

            navMock.Verify(n => n.NavigateTo(It.IsAny<BaseViewModel>()), Times.Never);
        }

        [Fact]
        public void OpenDetailsStation_AdminAvecStation_DoitNaviguer()
        {
            var userDalMock = new Mock<IUserDAL>();
            var navMock = new Mock<INavigationService>();
            var sessionMock = new Mock<IUserSessionService>();
            var stationMock = new Mock<IStationService>();
            var messageMock = new Mock<IMessageService>();

            var admin = new User { Id = 1, Role = UserRole.Administrateur, StationId = 10 };

            sessionMock.Setup(s => s.ConnectedUser).Returns(admin);

            userDalMock.Setup(d => d.GetUserWithStation(1))
                       .Returns(new User { Id = 1, StationId = 10, Station = new Station { IdStation = 10 } });

            var vm = new AdminHomeViewModel(
                userDalMock.Object,
                navMock.Object,
                sessionMock.Object,
                stationMock.Object,
                messageMock.Object
            );

            vm.DetailsStationCommand.Execute(null);

            navMock.Verify(n => n.NavigateTo(It.IsAny<BaseViewModel>()), Times.Once);
        }

    }
}
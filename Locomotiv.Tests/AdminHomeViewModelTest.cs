using Locomotiv.Model;
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
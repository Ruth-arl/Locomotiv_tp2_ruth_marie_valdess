using Locomotiv.Model;
using Locomotiv.Model.Enums;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.ViewModel;
using Moq;
using Xunit;

namespace Locomotiv.Tests
{
    public class StationDetailsViewModelTest
    {
        [Fact]
        public void Constructeur_AvecUtilisateurAyantStation_LoadLaStation()
        {
            var mockStationService = new Mock<IStationService>();
            var mockSession = new Mock<IUserSessionService>();
            var mockNav = new Mock<INavigationService>();
            var mockMessage = new Mock<IMessageService>();

            var station = new Station
            {
                IdStation = 5,
                Nom = "Gare Test",
                Trains = new List<Train>
                {
                    new Train { EstEnGare = true },
                    new Train { EstEnGare = false }
                },
                Voies = new List<Voie>
                {
                    new Voie { Nom = "Voie 1" },
                    new Voie { Nom = "Voie 2" }
                },
                Signaux = new List<Signal>
                {
                    new Signal { Type = "Rouge", Etat = SignalState.Rouge }
                }
            };

            mockSession.Setup(s => s.ConnectedUser)
                       .Returns(new User { StationId = 5 });

            mockStationService.Setup(s => s.GetStationById(5))
                              .Returns(station);

            var vm = new StationDetailsViewModel(
                mockStationService.Object,
                mockSession.Object,
                mockNav.Object,
                mockMessage.Object
            );

            Assert.NotNull(vm.Station);
            Assert.Equal("Gare Test", vm.Station.Nom);
            Assert.Equal(2, vm.Trains.Count);
            Assert.Equal(2, vm.Voies.Count);
            Assert.Equal(1, vm.Signaux.Count);
            Assert.Single(vm.TrainsEnGare);
        }

        [Fact]
        public void Constructeur_SansUtilisateurConnecte_MessageErreurEtStationNull()
        {
            var mockStationService = new Mock<IStationService>();
            var mockSession = new Mock<IUserSessionService>();
            var mockNav = new Mock<INavigationService>();
            var mockMessage = new Mock<IMessageService>();

            mockSession.Setup(s => s.ConnectedUser)
                       .Returns((User?)null);

            var vm = new StationDetailsViewModel(
                mockStationService.Object,
                mockSession.Object,
                mockNav.Object,
                mockMessage.Object
            );

            Assert.Null(vm.Station);

            mockMessage.Verify(m => m.ShowError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Constructeur_UtilisateurSansStation_AssigneMessage()
        {
            var mockStationService = new Mock<IStationService>();
            var mockSession = new Mock<IUserSessionService>();
            var mockNav = new Mock<INavigationService>();
            var mockMessage = new Mock<IMessageService>();

            mockSession.Setup(s => s.ConnectedUser)
                .Returns(new User { StationId = null });

            var vm = new StationDetailsViewModel(
                mockStationService.Object,
                mockSession.Object,
                mockNav.Object,
                mockMessage.Object
            );

            Assert.Null(vm.Station);

            mockMessage.Verify(m => m.Show("Vous n'êtes assigné(e) à aucune station."), Times.Once);
        }
    }
}

using static System.Collections.Specialized.BitVector32;
using Xunit;
using Locomotiv.Model.Interfaces;
using Locomotiv.Model;
using Locomotiv.ViewModel;
using Moq;
using Locomotiv.Model.Enums;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Locomotiv.Tests
{
    public class StationViewModelTests
    {
        [Fact]
        public void AjouterTrain_Dans_Station_Tant_Que_Capacite_Pas_Atteinte()
        {
            // ARRANGE
            var mockTrainDal = new Mock<ITrainDAL>();
            var mockStationService = new Mock<IStationService>();
            var mockSession = new Mock<IUserSessionService>();
            mockSession.Setup(s => s.ConnectedUser).Returns(new User { Role = UserRole.Administrateur });
            var mockNav = new Mock<INavigationService>();

            var station = new Station
            {
                IdStation = 1,
                CapaciteMax = 5,
                Trains = new List<Train>()
            };

            mockStationService.Setup(s => s.GetAllStations()).Returns(new List<Station> { station });

            var vm = new StationViewModel(
                mockStationService.Object,
                mockSession.Object,
                mockNav.Object,
                mockTrainDal.Object
            );

            var nouveauTrain = new Train { IdStation = station.IdStation };
            vm.SelectedStation = station;
            vm.NouveauTrain = nouveauTrain;

            mockTrainDal.Setup(x => x.AjouterTrain(It.IsAny<Train>()))
                        .Callback<Train>(t => station.Trains.Add(t));

            // ACT
            vm.AddTrainCommand.Execute(null);

            // ASSERT
            mockTrainDal.Verify(x => x.AjouterTrain(nouveauTrain), Times.Once);
            Assert.Contains(nouveauTrain, station.Trains);
            Assert.True(station.Trains.Count <= station.CapaciteMax);

        }

        [Fact]
        public void SupprimerTrain_De_Station_Si_Train_Selectionne()
        {
            // ARRANGE
            var mockTrainDal = new Mock<ITrainDAL>();
            var mockStationService = new Mock<IStationService>();
            var mockSession = new Mock<IUserSessionService>();
            mockSession.Setup(s => s.ConnectedUser).Returns(new User { Role = UserRole.Administrateur });
            var mockNav = new Mock<INavigationService>();

            var train = new Train ();

            var station = new Station
            {
                CapaciteMax = 5,
                Trains = new List<Train> { train }
            };

            mockStationService.Setup(s => s.GetAllStations()).Returns(new List<Station> { station });

            var viewModel = new StationViewModel(
                mockStationService.Object,
                mockSession.Object,
                mockNav.Object,
                mockTrainDal.Object
            );

            viewModel.SelectedStation = station;
            viewModel.SelectedTrain = train;

            mockTrainDal.Setup(x => x.SupprimerTrain(It.IsAny<Train>()))
                        .Callback<Train>(t => station.Trains.Remove(t));

            // ACT
            viewModel.DeleteTrainCommand.Execute(null);

            // ASSERT
            mockTrainDal.Verify(x => x.SupprimerTrain(train));
            Assert.False(station.Trains.Contains(train));

        }

    }
}
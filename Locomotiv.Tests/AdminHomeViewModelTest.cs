using static System.Collections.Specialized.BitVector32;
using Xunit;

namespace Locomotiv.Tests
{
    public class AdminHomeViewModelTest
    {
        private readonly AdminHomeViewModel _vm;
        [Fact]
        public void Constructeur_DoitsInitialiserLaListeDesStations()
        {
            var stationService = new Mock<IStationService>();
            
        }

        [Fact]
        public void Test2()
        {
        }
    }
}
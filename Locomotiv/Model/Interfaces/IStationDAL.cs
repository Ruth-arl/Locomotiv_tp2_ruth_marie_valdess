
namespace Locomotiv.Model.DAL
{
    public interface IStationDAL
    {
        IEnumerable<Station> GetAll();
        List<Station> GetAllStations();
        Station GetStationDetails(int id);

        IEnumerable<Train> GetTrainsByStation(int IdStation);

        IEnumerable<Voie> GetVoiesByStation(int IdStation);

        IEnumerable<Signal> GetSignauxByStation(int IdStation);
    }
}
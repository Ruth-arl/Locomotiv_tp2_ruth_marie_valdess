
namespace Locomotiv.Model.DAL
{
    public interface IStationDAL
    {
        List<Station> GetAllStations();
        Station GetStationDetails(int id);
    }
}
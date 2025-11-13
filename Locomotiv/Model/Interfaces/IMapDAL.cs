namespace Locomotiv.Model.Interfaces
{
    public interface IMapDAL
    {
        List<Block> GetAllBlocks();
        List<PointInteret> GetAllPonitInteret();
        List<Station> GetAllStations();
        void UpdateBlockOccupancy(int blockId, int? trainId);
    }
}
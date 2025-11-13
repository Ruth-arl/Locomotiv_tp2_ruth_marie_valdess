using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class StationDAL : IStationDAL
    {
        private readonly ApplicationDbContext _context;

        public StationDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Station> GetAllStations()
        {
            return _context.Stations.ToList();
        }

        public Station GetStationDetails(int id)
        {
            return _context.Stations
                .Include(s => s.Voies)
                .Include(s => s.Signaux)
                .Include(s => s.Trains)
                .FirstOrDefault(s => s.IdStation == id);
        }
    }
}

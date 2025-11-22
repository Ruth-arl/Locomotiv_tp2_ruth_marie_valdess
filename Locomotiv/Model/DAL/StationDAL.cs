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

        public IEnumerable<Station> GetAll()
        {
            return _context.Stations
                .Include(s => s.Trains)
                .Include(s => s.Voies)
                .Include(s => s.Signaux)
                .ToList();
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

        public IEnumerable<Train> GetTrainsByStation(int stationId)
        {
            return _context.Trains
                .Where(t => t.IdStation == stationId)
                .ToList();
        }

        public IEnumerable<Voie> GetVoiesByStation(int stationId)
        {
            return _context.Voies
                .Where(v => v.IdStation == stationId)
                .ToList();
        }


        public IEnumerable<Signal> GetSignauxByStation(int stationId)
        {
            return _context.Signals
                .Where(s => s.IdStation == stationId)
                .ToList();
        }
    }
}

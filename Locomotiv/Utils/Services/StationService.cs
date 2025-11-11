using Locomotiv.Model;
using Locomotiv.Utils.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Locomotiv.Utils.Services
{
    public class StationService : IStationService
    {
        private readonly ApplicationDbContext _context;

        public StationService(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public List<Station> GetAllStations()
        {
            return _context.Stations
                .Include(s => s.Trains)
                .Include(s => s.Voies)
                .Include(s => s.Signaux)
                .ToList();
        }

        
        public Station? GetStationById(int id)
        {
            return _context.Stations
                .Include(s => s.Trains)
                .Include(s => s.Voies)
                .Include(s => s.Signaux)
                .FirstOrDefault(s => s.IdStation == id);
        }
    }
}

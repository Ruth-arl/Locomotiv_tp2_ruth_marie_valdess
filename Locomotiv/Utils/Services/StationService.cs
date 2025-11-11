using Locomotiv.Model;
using Locomotiv.Utils.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                   .ToList();
        }
    }
}

using Locomotiv.Model.Enums;
using Locomotiv.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class MapDAL : IMapDAL
    {
        private readonly ApplicationDbContext _context;

        public MapDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Station> GetAllStations()
        {
            return _context.Stations
                .Include(s => s.Trains)
                .ToList();
        }

        public List<PointInteret> GetAllPonitInteret()
        {
            return _context.PointsInteret.ToList();
        }

        public List<Block> GetAllBlocks()
        {
            return _context.Blocks
                .Include(b => b.TrainOccupant)
                .ToList();
        }

        public void UpdateBlockOccupancy(int blockId, int? trainId)
        {
            var block = _context.Blocks.Find(blockId);
            if (block != null)
            {
                block.TrainOccupantId = trainId;
                block.EtatSignal = trainId.HasValue ? SignalState.Rouge : SignalState.Vert;
                _context.SaveChanges();
            }
        }
    }
}

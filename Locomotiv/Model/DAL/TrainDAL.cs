using Locomotiv.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
   public class TrainDAL : ITrainDAL
    {
        private readonly ApplicationDbContext _context;

        public TrainDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddTrain(Train train)
        {

            _context.Trains.Add(train);
            _context.SaveChanges();
        }

        public void DeleteTrain(Train train)
        {
            _context.Trains.Remove(train);
            _context.SaveChanges();
        }

        public List<Train> GetTrains()
        {
            return _context.Trains.ToList();
        }


        public List<Train> GetTrainsByStationId(int stationId)
        {
            return _context.Trains
                .Where(t => t.IdStation == stationId)
                .ToList();
        }
    }
}

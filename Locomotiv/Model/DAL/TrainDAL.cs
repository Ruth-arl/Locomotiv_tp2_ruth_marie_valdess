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
   public class TrainDAL : ITrainDAL
    {
        private readonly ApplicationDbContext _context;

        public TrainDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Train> GetAll()
        {
            return _context.Trains
                .Include(t => t.Station)
                .ToList();
        }

        public Train? GetById(int id)
        {
            return _context.Trains
                .Include(t => t.Station)
                .FirstOrDefault(t => t.IdTrain == id);
        }

        public IEnumerable<Train> GetTrainsByStatus(TrainStatus status)
        {
            return _context.Trains
                .Include(t => t.Station)
                .Where(t => t.Etat == status)
                .ToList();
        }

        /// <summary>
        /// Récupère tous les trains d'une station
        /// </summary>
        /// <param name="stationId">Identifiant de la station</param>
        /// <returns>Collection des trains de la station</returns>
        public IEnumerable<Train> GetTrainsByStation(int stationId)
        {
            return _context.Trains
                .Where(t => t.IdStation == stationId)
                .ToList();
        }

        /// <summary>
        /// Vérifie si un train peut être ajouté à une station selon sa capacité maximale
        /// </summary>
        /// <param name="stationId">Identifiant de la station</param>
        /// <returns>True si la station peut accueillir un nouveau train, false sinon</returns>
        public bool CanAddTrainToStation(int stationId)
        {
            var station = _context.Stations
                .Include(s => s.Trains)
                .FirstOrDefault(s => s.IdStation == stationId);

            if (station == null)
                return false;

            var trainsEnGare = station.Trains.Count(t => t.Etat == TrainStatus.EnGare);
            return trainsEnGare < station.CapaciteMax;
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

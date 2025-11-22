using Locomotiv.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.Interfaces
{
    public interface ITrainDAL
    {
        IEnumerable<Train> GetAll();
        Train? GetById(int id);
        void AddTrain(Train train);
        void DeleteTrain(Train train);
        List<Train> GetTrains();
        IEnumerable<Train> GetTrainsByStatus(TrainStatus status);

        IEnumerable<Train> GetTrainsByStation(int stationId);

        bool CanAddTrainToStation(int stationId);
    }
}

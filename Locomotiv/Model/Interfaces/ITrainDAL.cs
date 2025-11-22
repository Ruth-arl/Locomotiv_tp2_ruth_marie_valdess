using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.Interfaces
{
    public interface ITrainDAL
    {
        void AddTrain(Train train);
        void DeleteTrain(Train train);
        List<Train> GetTrains();
    }
}

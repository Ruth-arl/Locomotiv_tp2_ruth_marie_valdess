using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.Interfaces
{
    public interface ITrainDAL
    {
        void AjouterTrain(Train train);
        void SupprimerTrain(Train train);
        List<Train> GetTrains();
    }
}

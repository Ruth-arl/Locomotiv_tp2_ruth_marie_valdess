using Locomotiv.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public  class Train
    {
        // Clé primaire
        public int IdTrain { get; set; }
        // Clé étrangère
        public int IdStation { get; set; }
        public Station Station { get; set; }
        public DateTime HeureDepart { get; set; }
        public DateTime HeureArrivee { get; set; }

        public TrainType Type { get; set; }
        public TrainStatus Etat { get; set; }

        public bool EstEnGare { get; set; }
    }
}

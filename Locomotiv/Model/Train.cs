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

        public TrainType Type { get; set; }
        public TrainStatus Etat { get; set; }

        public bool EstEnGare { get; set; }

        public string Locomotive { get; set; } = "Locomotive 1";

        public int NombreWagons { get; set; } = 1;
    }
}

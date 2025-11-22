using Locomotiv.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class Block
    {
        public int IdBlock { get; set; }
        public string Nom { get; set; } 
        public int? TrainOccupantId { get; set; }
        public Train? TrainOccupant { get; set; }
        public bool EstOccupe => TrainOccupantId != null;
        public SignalState EtatSignal { get; set; }
        public double LatitudeDebut { get; set; }
        public double LongitudeDebut { get; set; }
        public double LatitudeFin { get; set; }
        public double LongitudeFin { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class Voie
    {
        public int IdVoie { get; set; }
        public string? Nom { get; set; }
        public string Numero { get; set; }
        public int IdStation { get; set; }
        public Station? Station { get; set; }
        public bool EstOccupee { get; set; }
        public int? IdTrainActuel { get; set; }
        public Train? TrainActuel { get; set; }
    }
}

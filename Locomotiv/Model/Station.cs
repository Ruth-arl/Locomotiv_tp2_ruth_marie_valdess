using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Locomotiv.Model
{
    public class Station
    {
        // Clé primaire
        public int IdStation { get; set; }
        public string ?Nom { get; set; }
        public string ?Ville { get; set; }
        public int CapaciteMax { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public ICollection<Train> Trains { get; set; } = new List<Train>();
        public ICollection<Voie> Voies { get; set; } = new List<Voie>();
        public ICollection<Signal> Signaux { get; set; } = new List<Signal>();
        public Station()
        {
        }
   
    }
}

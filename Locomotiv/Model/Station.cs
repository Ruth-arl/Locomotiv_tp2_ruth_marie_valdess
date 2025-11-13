using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class Station
    {
        // Clé primaire
        public int IdStation { get; set; }
        public string ?Nom { get; set; }
        public string ?Ville { get; set; }

        public int CapaciteMax { get; set; }

        // Relation : une station possède plusieurs trains
        public ICollection<Train> Trains { get; set; } = new List<Train>();

        public Station()
        {
        }

        public List<Voie> Voies { get; set; } = new List<Voie>();
        public List<Signal> Signaux { get; set; } = new List<Signal>();
       
    }
}

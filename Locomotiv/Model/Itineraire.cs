using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class Itineraire
    {
        public int IdItineraire { get; set; }
        public int IdTrain { get; set; }
        public Train Train { get; set; }
        public DateTime HeureDepart { get; set; }
        public DateTime HeureArriveeEstimee { get; set; }
        public ICollection<ArretItineraire> Arrets { get; set; } = new List<ArretItineraire>();
        public ICollection<Block> BlocksTraverses { get; set; } = new List<Block>();}

    public class ArretItineraire
    {    
        public int IdArret { get; set; }
        public int IdItineraire { get; set; }
        public Itineraire Itineraire { get; set; }
        public int Ordre { get; set; }
        public int? IdStation { get; set; }
        public Station? Station { get; set; }
        public int? IdPointInteret { get; set; }
        public PointInteret? PointInteret { get; set; }
        public DateTime HeureArrivee { get; set; }
        public DateTime HeureDepart { get; set; }
        public int DureeArret { get; set; }
    }
}

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
        public int Id { get; set; }
        public string Nom { get; set; }
        public int? StationId { get; set; }
        public Station? Station { get; set; }
        public int? TrainOccupantId { get; set; }
        public Train? TrainOccupant { get; set; }
        public SignalState EtatSignal { get; set; }
        public List<GMap.NET.PointLatLng> Coordonnees { get; set; }
        public ICollection<PointInteret> PointInterets { get; set; } = new List<PointInteret>();

    }
}

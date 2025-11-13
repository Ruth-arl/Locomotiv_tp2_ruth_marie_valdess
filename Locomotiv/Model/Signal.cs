using Locomotiv.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class Signal
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public SignalState Etat { get; set; }
        public int StationId { get; set; }
        public Station Station { get; set; }
    }
}

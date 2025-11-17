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
        public string Type { get; set; } = string.Empty;
        public SignalState Etat { get; set; } = SignalState.Vert;

        public int IdStation { get; set; }
        public Station? Station { get; set; }
    }
}

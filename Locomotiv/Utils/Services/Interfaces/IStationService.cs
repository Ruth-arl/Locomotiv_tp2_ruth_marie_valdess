using Locomotiv.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IStationService
    {
        List<Station> GetAllStations();
        Station? GetStationById(int id);
    }

}

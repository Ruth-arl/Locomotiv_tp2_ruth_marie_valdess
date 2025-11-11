using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Locomotiv.ViewModel;

namespace Locomotiv.ViewModel
{
    public class StationDetailsViewModel : BaseViewModel
    {
        public Station Station { get; }

        public ObservableCollection<Train> Trains { get; }
        public ObservableCollection<Voie> Voies { get; }
        public ObservableCollection<Signal> Signaux { get; }
        public ObservableCollection<Train> TrainsEnGare { get; }

        public ICommand RetourCommand { get; }

        public StationDetailsViewModel(Station station, System.Action retourAction)
        {
            Station = station;

            Trains = new ObservableCollection<Train>(station.Trains);
            Voies = new ObservableCollection<Voie>(station.Voies);
            Signaux = new ObservableCollection<Signal>(station.Signaux);
            TrainsEnGare = new ObservableCollection<Train>(station.Trains.Where(t => t.EstEnGare));

            RetourCommand = new RelayCommand(() => retourAction?.Invoke());
        }
    }
}

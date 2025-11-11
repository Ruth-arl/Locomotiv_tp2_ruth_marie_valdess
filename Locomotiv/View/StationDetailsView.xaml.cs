using Locomotiv.Model;
using Locomotiv.Utils.Services;
using Locomotiv.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NavSvc = Locomotiv.Utils.Services.NavigationService;

namespace Locomotiv.View
{
    public partial class StationDetailsView : UserControl
    {
        public StationDetailsView(Station station, System.Action retourAction)
        {
            InitializeComponent();
            DataContext = new StationDetailsViewModel(station, retourAction);
        }
    }
}

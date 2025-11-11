using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.ViewModel;

namespace Locomotiv.Utils.Services
{
    public class NavigationService : BaseViewModel, INavigationService
    {
        private readonly Func<Type, BaseViewModel> _viewModelFactory;
        private BaseViewModel _currentView;

        public BaseViewModel CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public event Action OnCurrentViewModelChanged;

        public NavigationService(Func<Type, BaseViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
        }

        public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            BaseViewModel viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewModel;
            OnCurrentViewModelChanged?.Invoke();
        }
    }
}

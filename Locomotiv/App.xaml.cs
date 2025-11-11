using System;
using System.IO;
using System.Windows;
using Locomotiv.Utils;
using Locomotiv.ViewModel;
using Locomotiv.View;
using Microsoft.Extensions.DependencyInjection;
using Locomotiv.Model.Interfaces;
using Locomotiv.Model.DAL;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Locomotiv
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
            IConfiguration configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<ConnectUserViewModel>();

            services.AddSingleton<IUserDAL, UserDAL>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IUserSessionService, Service>();
            services.AddSingleton<IStationService, StationService>();
            services.AddTransient<StationViewModel>();


            services.AddSingleton<Func<Type, BaseViewModel>>(serviceProvider =>
            {
                BaseViewModel ViewModelFactory(Type viewModelType)
                {
                    return (BaseViewModel)serviceProvider.GetRequiredService(viewModelType);
                }
                return ViewModelFactory;
            });

            services.AddDbContext<ApplicationDbContext>();

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.EnsureCreated();  
                dbContext.SeedData();                
            }

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}

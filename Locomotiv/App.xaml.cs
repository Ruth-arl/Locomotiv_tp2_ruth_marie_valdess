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
            services.AddTransient<StationViewModel>();
            services.AddTransient<StationDetailsViewModel>();
            services.AddSingleton<AdminHomeViewModel>();
            services.AddSingleton<IUserSessionService, UserSessionService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddTransient<AdminHomeViewModel>();

            services.AddScoped<IUserDAL, UserDAL>();
            services.AddSingleton<IStationService, StationService>();
            services.AddSingleton<IUserSessionService, Service>();
            services.AddScoped<ITrainDAL, TrainDAL>();
            services.AddScoped<IStationDAL, StationDAL>();


            services.AddSingleton<Func<Type, BaseViewModel>>(serviceProvider =>
            {
                return new Func<Type, BaseViewModel>(viewModelType =>
                {
                    var stationService = serviceProvider.GetRequiredService<IStationService>();
                    var userSession = serviceProvider.GetRequiredService<IUserSessionService>();
                    var navigation = serviceProvider.GetRequiredService<INavigationService>();
                    var userDal = serviceProvider.GetRequiredService<IUserDAL>();
                    var trainDal = serviceProvider.GetRequiredService<ITrainDAL>();
                    var messageService = serviceProvider.GetRequiredService<IMessageService>();
                    var stationDal = serviceProvider.GetRequiredService<IStationDAL>();

                    if (viewModelType == typeof(StationViewModel))
                    {
                        return new StationViewModel(stationService, userSession, stationDal, navigation, trainDal);
                    }

                    if (viewModelType == typeof(StationDetailsViewModel))
                    {
                        return new StationDetailsViewModel(stationService, userSession, navigation, messageService);
                    }

                    if (viewModelType == typeof(HomeViewModel))
                    {
                        return serviceProvider.GetRequiredService<HomeViewModel>();
                    }

                    if (viewModelType == typeof(ConnectUserViewModel))
                    {
                        return (BaseViewModel)Activator.CreateInstance(typeof(ConnectUserViewModel), userDal, navigation, userSession);
                    }

                    return (BaseViewModel)serviceProvider.GetRequiredService(viewModelType);
                });
            });


            services.AddSingleton<INavigationService, NavigationService>();

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

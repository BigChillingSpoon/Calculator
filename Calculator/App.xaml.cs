using Calculator.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Calculator.Core.Interfaces;
using Calculator.Core;
using Calculator.Services.Interfaces;
using Calculator.Services;
using Calculator.IO.Services.Interfaces;
using Calculator.IO.Services;
using Calculator.AppLayer.Services;
using Calculator.AppLayer.Interfaces;
using Calculator.Extensions;
using Calculator.Core.Services;
using Calculator.AppLayer.Validators;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            UnhandledExceptionHandler.Register();
            var services = ConfigureServices();
            var mainWindow = services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services
                .AddPresentationServices()
                .AddApplicationServices()
                .AddCoreServices()
                .AddInfrastructureServices();

            return services.BuildServiceProvider();
        }
    }

}

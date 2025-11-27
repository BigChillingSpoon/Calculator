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
using Calculator.AppLayer.Services.Interfaces;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = ConfigureServices();
            var mainWindow = services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<INotifyService, NotifyService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IFileService, FileService>();
            
            ////APPLAYER
            services.AddSingleton<IFileProcessingService, FileProcessingService>();

            //// CORE - TODO presunout do extensionu v core
            services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();

            //// Registrace CORE vrstvy
            //services.AddSingleton<ICalculatorService, CalculatorService>();

            // Registrace ViewModelů
            services.AddTransient<MainViewModel>();

            // Registrace okna
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }
    }

}

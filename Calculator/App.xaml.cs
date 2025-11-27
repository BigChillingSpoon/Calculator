using Calculator.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

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

            //// Registrace IO vrstvy
            //services.AddSingleton<IFileService, FileService>();

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

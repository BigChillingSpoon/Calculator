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

            //WPF
            services.AddTransient<INotifyService, NotifyService>();
            services.AddTransient<IDialogService, DialogService>();

            //APPLAYER
            services.AddSingleton<IFileProcessingService, FileProcessingService>();
            services.AddSingleton<IEvaluationProcessingService, EvaluationProcessingService>(); 

            // CORE - TODO presunout do extensionu v core
            services.AddSingleton<IExpressionEvaluationService, ExpressionEvaluationService>();
            services.AddSingleton<IExpressionValidator, ExpressionValidator>();
            services.AddSingleton<IExpressionTokenizer, ExpressionTokenizer>();
            services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();
            services.AddSingleton<IExpressionNormalizer, ExpressionNormalizer>();
            services.AddSingleton<ISignNormalizer, SignNormalizer>();
            services.AddSingleton<IUnaryMerger, UnaryMerger>();
            services.AddSingleton<IUnaryClassifier, UnaryClassifier>();
            //IO 
            services.AddSingleton<IFileService, FileService>();
            
            //VM
            services.AddTransient<MainViewModel>();

            //WINDOW
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }
    }

}

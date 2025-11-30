using Calculator.AppLayer.Interfaces;
using Calculator.AppLayer.Services;
using Calculator.AppLayer.Validators;
using Calculator.Core.Interfaces;
using Calculator.Core.Services;
using Calculator.Core;
using Calculator.IO.Services.Interfaces;
using Calculator.IO.Services;
using Calculator.Services.Interfaces;
using Calculator.Services;
using Calculator.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            services.AddTransient<INotifyService, NotifyService>();
            services.AddTransient<IDialogService, DialogService>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileProcessingService, FileProcessingService>();
            services.AddSingleton<IEvaluationProcessingService, EvaluationProcessingService>();
            services.AddSingleton<IUserFileInputValidator, UserFileInputValidator>();
            return services;
        }

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<IExpressionEvaluationService, ExpressionEvaluationService>();
            services.AddSingleton<IExpressionValidator, ExpressionValidator>();
            services.AddSingleton<IExpressionNormalizer, ExpressionNormalizer>();
            services.AddSingleton<IExpressionTokenizer, ExpressionTokenizer>();
            services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();
            services.AddSingleton<IUnaryClassifier, UnaryClassifier>();
            services.AddSingleton<IUnaryMerger, UnaryMerger>();
            services.AddSingleton<ISignNormalizer, SignNormalizer>();
            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>();
            return services;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calculator.Extensions
{
    internal class UnhandledExceptionHandler
    {
        public static void Register()
        {
            // WPF UI thread
            Application.Current.DispatcherUnhandledException += (s, e) =>
            {
                Handle(e.Exception);
                e.Handled = true;
            };

            // Task exceptions
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Handle(e.Exception);
                e.SetObserved();
            };

            // Non-UI exceptions
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Handle(e.ExceptionObject as Exception);
            };
        }

        private static void Handle(Exception? ex)
        {
            // TODO: LOG
            // TODO: Show custom dialog

            MessageBox.Show(
                ex?.Message ?? "Unknown critical error",
                "Unexpected Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

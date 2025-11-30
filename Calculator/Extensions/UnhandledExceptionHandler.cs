using System;
using System.Collections.Generic;
using System.IO;
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
            Log(ex);
           
            MessageBox.Show(
                "Unknown critical error",
                "Unexpected Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private static void Log(Exception? ex)
        {
            try
            {
                var tempPath = Path.Combine(
                    Path.GetTempPath(),
                    "Calculator_crash.log"); // we dont have to create unique name here

                File.AppendAllText(
                    tempPath,
                    $"{DateTime.Now}: {ex}\n\n");
            }
            catch
            {
                // We do not log logger errors ever.
            }
        }
    }
}

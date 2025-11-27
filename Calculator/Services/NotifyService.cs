using Calculator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace Calculator.Services
{
    public class NotifyService : INotifyService
    {
        public void ShowError(string message) => MessageBox.Show(message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        public void ShowInfo(string message) => MessageBox.Show(message, "Info", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        public void ShowWarning(string message) => MessageBox.Show(message, "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
    }
}

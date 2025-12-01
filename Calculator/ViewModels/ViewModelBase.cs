using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(name);   
            return true;
        }

        //for UI blocking during async operations
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        private string _busyStatusMessage = string.Empty;
        public string BusyStatusMessage
        {
            get => _busyStatusMessage;
            set => SetProperty(ref _busyStatusMessage, value);
        }

        protected void SetBusyStatus(bool isBusy, string? message = null)
        {
            IsBusy = isBusy;
            if (isBusy && message != null)
                BusyStatusMessage = message;
            else if (!isBusy)
                BusyStatusMessage = string.Empty;
        }
    }
}

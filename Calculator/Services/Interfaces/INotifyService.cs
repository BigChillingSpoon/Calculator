using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Services.Interfaces
{
    public interface INotifyService
    {
        public void ShowError(string message);
        public void ShowInfo(string message);
        public void ShowWarning(string message);
    }
}

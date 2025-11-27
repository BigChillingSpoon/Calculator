using Calculator.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Services
{
    public class DialogService : IDialogService
    { 
        public bool ShowOpenFileDialog(out string filePath)
        {
            var dialog = new OpenFileDialog();
            filePath = dialog.ShowDialog() == true ? dialog.FileName : String.Empty;
            return !String.IsNullOrEmpty(filePath);
        }
    }
}

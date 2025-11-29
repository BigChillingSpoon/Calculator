using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Services.Interfaces
{
    public interface IDialogService
    {
        public bool ShowOpenFileDialog(out string filePath);
        public bool ShowOpenDirectoryDialog(out string dirPath);
    }
}

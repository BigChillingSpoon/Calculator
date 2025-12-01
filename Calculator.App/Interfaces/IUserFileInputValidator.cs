using Calculator.AppLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.AppLayer.Interfaces
{
    public interface IUserFileInputValidator
    {
        public ProcessResult ProcessUserFileInputs(string inputPath, string outputDirPath, string outputFileName);
    }
}

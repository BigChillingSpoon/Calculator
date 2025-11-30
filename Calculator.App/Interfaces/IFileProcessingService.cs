using Calculator.AppLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.AppLayer.Interfaces
{
    public interface IFileProcessingService
    {
        Task<ProcessResult> ProcessEvaluationFromFileAsync(string inputFilePath, string outputDirectoryPath, string outputFileName); 
    }
}

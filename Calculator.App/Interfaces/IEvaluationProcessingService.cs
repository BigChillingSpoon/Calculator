using Calculator.AppLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.AppLayer.Interfaces
{
    public interface IEvaluationProcessingService
    {
        public ProcessResult ProcessEvaluation(string expression);
    }
}

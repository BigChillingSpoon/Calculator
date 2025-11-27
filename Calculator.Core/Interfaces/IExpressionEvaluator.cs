using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Interfaces
{
    public interface IExpressionEvaluator
    {
        public ParsingResult Evaluate(string expression);
    }
}

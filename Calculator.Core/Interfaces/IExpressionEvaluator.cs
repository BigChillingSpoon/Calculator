using Calculator.Core.Models;
using Calculator.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Interfaces
{
    public interface IExpressionEvaluator
    {
        public ParsingResult EvaluateExpression(List<IExpressionToken> tokens);
    }
}

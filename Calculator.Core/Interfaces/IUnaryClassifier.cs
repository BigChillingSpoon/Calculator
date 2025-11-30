using Calculator.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Interfaces
{
    public interface IUnaryClassifier
    {
        public List<IExpressionToken> ClassifyUnaryOperators(List<IExpressionToken> expressionTokens);
    }
}

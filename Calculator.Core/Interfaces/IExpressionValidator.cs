using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Interfaces
{
    public interface IExpressionValidator
    {
        public ParsingResult ValidateExpression(string expression);
    }
}

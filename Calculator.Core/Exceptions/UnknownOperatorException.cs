using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Exceptions
{
    public class UnknownOperatorException : Exception
    {
        public UnknownOperatorException(string operatorValue)
            : base($"Unknown operator encountered: {operatorValue}")
        {
        }
    }
}

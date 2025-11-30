using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Exceptions
{
    public class InvalidUnaryPlacementException : Exception
    {
        public InvalidUnaryPlacementException(string message)
            : base($"Invalid placement of unary operator: {message}")
        {
        }
    }
}

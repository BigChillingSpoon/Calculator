using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Exceptions
{
    public class InvalidUnaryPlacementException : Exception
    {
        /// <summary>
        /// Signalizes that unary operator is placed incorrectly in the expression.
        /// </summary>
        /// <param name="message"></param>
        public InvalidUnaryPlacementException(string message)
            : base($"Invalid placement of unary operator: {message}")
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Exceptions
{
    public class UnsupportedCharacterException : Exception
    {
        /// <summary>
        /// Signalizes that expression contains one or more unsupported characters.
        /// </summary>
        public UnsupportedCharacterException()
            : base($"Expression contains one or more unsupported characters")
        {
        }
    }
}

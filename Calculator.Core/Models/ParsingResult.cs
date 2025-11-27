using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Models
{
    public class ParsingResult
    {
        public bool Success { get; set; }
        public double Value { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

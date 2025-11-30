using Calculator.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Models
{
    public class ParsingResult
    {
        public bool Success { get; set; }
        public string Value { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public ErrorTypeCore ErrorType { get; set; } = ErrorTypeCore.None;
    }
}

using Calculator.AppLayer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.AppLayer.Models
{
    public class ProcessResult
    {
        public bool Success { get; set; }
        public double Value { get; set; } 
        public string ErrorMessage { get; set; } = String.Empty;
        public ErrorType ErrorType { get; set; } = ErrorType.None;
    }
}

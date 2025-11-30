using Calculator.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Models.Interfaces
{
    public interface IExpressionToken
    {
        public ExpressionTokenType TokenType { get; }
        public string RawValue { get; set; }
    }
}

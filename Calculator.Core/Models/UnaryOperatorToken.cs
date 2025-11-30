using Calculator.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Models
{
    public class UnaryOperatorToken : OperatorToken
    {
        public UnaryOperatorToken(OperationType operationType) : base(operationType)
        { 
        }
        public override string ToString()
        {
            return $"Unary Operator token: {RawValue}";
        }
    }
}

using Calculator.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Models
{
    public class BinaryOperatorToken : OperatorToken
    {
        public BinaryOperatorToken(OperationType operationType) : base(operationType)
        {
        }
        public override string ToString()
        {
            return $"Binary Operator token: {RawValue}";
        }
    }
}

using Calculator.Core.Models.Enums;
using Calculator.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Models
{
    public class NumericToken : IExpressionToken
    {
        public ExpressionTokenType TokenType => ExpressionTokenType.Number;
        public string RawValue { get; set; } = String.Empty;
        public BigInteger NumericValue { get; set; }
        
        public NumericToken(string value)
        {
            RawValue = value;
            NumericValue = BigInteger.Parse(value);
        }

        public NumericToken(BigInteger value)
        {
            NumericValue = value;
            RawValue = value.ToString();
        }
    }
}

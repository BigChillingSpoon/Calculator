using Calculator.Core.Exceptions;
using Calculator.Core.Models.Enums;
using Calculator.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Models
{
    public class OperatorToken : IExpressionToken
    {
        public ExpressionTokenType TokenType => ExpressionTokenType.Operator;
        public string RawValue { get; set; } = String.Empty;
        public OperationType OperationType { get; set; }

        public OperatorToken(string value)
        {
            RawValue = value;
            OperationType = value switch
            {
                "+" => OperationType.Addition,
                "-" => OperationType.Subtraction,
                "*" => OperationType.Multiplication,
                "/" => OperationType.Division,
                _ => throw new UnknownOperatorException(value)
            };
        }

        public OperatorToken(OperationType operationType)
        {
            OperationType = operationType;
            RawValue = operationType.ToSymbol();
        }

        public override string ToString()
        {
            return $"Operator token: {RawValue}";
        }
    }
}

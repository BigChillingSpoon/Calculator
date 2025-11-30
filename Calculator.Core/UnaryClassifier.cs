using Calculator.Core.Interfaces;
using Calculator.Core.Models.Enums;
using Calculator.Core.Models;
using Calculator.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core
{
    public class UnaryClassifier : IUnaryClassifier
    {
        public List<IExpressionToken> ClassifyUnaryOperators(List<IExpressionToken> expressionTokens)
        {
            var output = new List<IExpressionToken>();
            bool expectUnary = true;

            foreach (var token in expressionTokens)
            {
                if (token is OperatorToken op)
                {
                    if (expectUnary && (op.OperationType == OperationType.Addition ||
                                        op.OperationType == OperationType.Subtraction))
                    {
                        // Unary operator
                        output.Add(new UnaryOperatorToken(op.OperationType));
                    }
                    else
                    {
                        // Binary operator
                        output.Add(new BinaryOperatorToken(op.OperationType));
                    }

                    expectUnary = true;
                }
                else
                {
                    // Numeric token
                    output.Add(token);
                    expectUnary = false;
                }
            }

            return output;
        }
    }
    
}

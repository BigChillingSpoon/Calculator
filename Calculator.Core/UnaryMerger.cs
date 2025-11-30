using Calculator.Core.Interfaces;
using Calculator.Core.Models.Enums;
using Calculator.Core.Models;
using Calculator.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Calculator.Core.Exceptions;

namespace Calculator.Core
{
    public class UnaryMerger : IUnaryMerger
    {
        public List<IExpressionToken> MergeUnaryOperators(List<IExpressionToken> tokens)
        {
            var output = new List<IExpressionToken>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is UnaryOperatorToken u)
                {
                    if (i + 1 >= tokens.Count || tokens[i + 1] is not NumericToken num)
                        throw new InvalidUnaryPlacementException("Unary operator must be followed by a number.");

                    // Apply unary
                    BigInteger value = num.NumericValue;
                    if (u.OperationType == OperationType.Subtraction)
                        value = -value;

                    output.Add(new NumericToken(value));
                    i++; // skip the number
                }
                else
                {
                    output.Add(tokens[i]);
                }
            }

            return output;
        }
    }
}

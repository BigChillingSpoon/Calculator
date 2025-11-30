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
    public class SignNormalizer : ISignNormalizer
    {
        /// <summary>
        /// Transforms sequence of sign operators(+,-) into just one sign operator for example --3 is normalized to +3
        /// Does not normalize other operators such as * or /
        /// </summary>
        /// <param name="expressionTokens"></param>
        /// <returns>Tokenized expression with normalized sign operators</returns>
        public List<IExpressionToken> NormalizeSigns(List<IExpressionToken> expressionTokens)
        {
            var normalizedTokens = new List<IExpressionToken>();
            var signBuffer = new List<OperatorToken>();

            foreach (var token in expressionTokens)
            {
                if (token is OperatorToken operatorToken)
                {
                    if (IsSignOperator(operatorToken))
                    {
                        // Collect + and -
                        signBuffer.Add(operatorToken);
                    }
                    else
                    {
                        // *, / -> must flush sign buffer and then add operator
                        FlushSignBufferTo(normalizedTokens, signBuffer);
                        normalizedTokens.Add(operatorToken);
                    }
                }
                else
                {
                    //flush signs then add number
                    FlushSignBufferTo(normalizedTokens, signBuffer);
                    normalizedTokens.Add(token);
                }
            }

            // End-of-stream flush to normalize trailing signs
            FlushSignBufferTo(normalizedTokens, signBuffer);

            return normalizedTokens;
        }

        private bool IsSignOperator(OperatorToken op)
        {
            return op.OperationType == OperationType.Addition || op.OperationType == OperationType.Subtraction;
        }

        private void FlushSignBufferTo(List<IExpressionToken> output, List<OperatorToken> signBuffer)
        {
            if (signBuffer.Count == 0)
                return;

            // If only one + or -, keep it
            if (signBuffer.Count == 1)
            {
                output.Add(signBuffer[0]);
            }
            else
            {
                // Compute final sign from + and -
                int subrtractionCount = signBuffer.Count(t => t.OperationType == OperationType.Subtraction);

                var finalType = (subrtractionCount % 2 == 0 ? OperationType.Addition : OperationType.Subtraction);

                output.Add(new OperatorToken(finalType));
            }

            signBuffer.Clear();
        }
    }
}

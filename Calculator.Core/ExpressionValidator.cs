using Calculator.Core.Exceptions;
using Calculator.Core.Interfaces;
using Calculator.Core.Models;
using Calculator.Core.Models.Enums;
using Calculator.Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core
{
    public class ExpressionValidator : IExpressionValidator
    {
        /// <summary>
        /// Decides which expressions are valid and which are not, based on syntax rules
        /// </summary>
        /// <param name="expressionTokens"></param>
        /// <returns>ParsingResult</returns>
        public ParsingResult ValidateExpression(List<IExpressionToken> expressionTokens)
        {
            if (expressionTokens == null || !expressionTokens.Any())
            {
                return new ParsingResult
                {
                    Success = false,
                    ErrorMessage = "Expression is empty",
                    ErrorType = ErrorTypeCore.Warning
                };
            }

            if (expressionTokens.Last() is OperatorToken)
            {
                return new ParsingResult
                {
                    Success = false,
                    ErrorMessage = "Expression cannot end with an operator",
                    ErrorType = ErrorTypeCore.Error
                };
            }

            if(ContainsInvalidSequenceOfOperators(expressionTokens))
            {
                return new ParsingResult
                {
                    Success = false,
                    ErrorMessage = "Expression contains invalid sequence of operators",
                    ErrorType = ErrorTypeCore.Error
                };
            }

            return new ParsingResult
            {
                Success = true
            };
        }

        /// <summary>
        /// Checks if the token list contains invalid sequences of operators (e.g., two operators in a row).
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>If contains returns true, otherwise returns false</returns>
        private bool ContainsInvalidSequenceOfOperators(List<IExpressionToken> tokens)
        {
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i] is OperatorToken firstOperator && tokens[i + 1] is OperatorToken secondOperator)
                {
                    return true;
                }
            }
            return false;
        }

    }
}

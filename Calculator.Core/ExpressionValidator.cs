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

        private bool ContainsInvalidSequenceOfOperators(List<IExpressionToken> tokens)
        {
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i] is OperatorToken firstOperator && tokens[i + 1] is OperatorToken secondOperator)
                {
                    return !IsOperatorSequenceValid(firstOperator, secondOperator);
                }
            }
            return false;
        }

        private bool IsOperatorSequenceValid(OperatorToken firstOperator, OperatorToken secondOperator)
        {
            var prevType = firstOperator.OperationType;
            var nextType = secondOperator.OperationType;

            bool prevIsSign = prevType == OperationType.Addition || prevType == OperationType.Subtraction;
            bool nextIsSign = nextType == OperationType.Addition || nextType == OperationType.Subtraction;

            //
            // Should be handled by normalizer, if not so, we handle it here
            //
            if (prevIsSign && nextIsSign)
                return true;

            //
            // 2) Binary operator followed by unary minus: OK
            //    * - -> VALID
            //    / - -> VALID
            //
            if (!prevIsSign && nextIsSign && nextType == OperationType.Subtraction)
                return true;

            //
            // Binary operator followed by unary plus, should be as well handled by normalizer, but if not so, we handle it here
            //
            if (!prevIsSign && nextIsSign && nextType == OperationType.Addition)
                return true;

            //
            // 4) Unary operator (- or +) cannot be followed by * or /
            //    - *   -> NON VALID
            //    + /   -> NON VALID
            //
            if (prevIsSign && !nextIsSign)
                return false;

            //
            // 5) Binary operator cannot follow another binary operator
            //    * * -> NON VALID
            //    / * -> NON VALID
            //
            if (!prevIsSign && !nextIsSign)
                return false;

            return true;
        }

    }
}

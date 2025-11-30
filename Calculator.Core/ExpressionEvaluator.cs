using Calculator.Core.Interfaces;
using Calculator.Core.Models;
using Calculator.Core.Models.Enums;
using Calculator.Core.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Calculator.Core
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        public ParsingResult EvaluateExpression(List<IExpressionToken> tokens)
        {
            // this is already handled by validator, but to be sure that this code is well testable 
            // and not dependent on any pipeline orchestration, we check it again
            if (tokens == null || tokens.Count == 0)
            {
                return new ParsingResult
                {
                    Success = false,
                    ErrorMessage = "Invalid Expression: Expression is empty",
                    ErrorType = ErrorTypeCore.Error
                };
            }

            BigInteger result = 0;
            BigInteger currentTerm = 0;

            OperationType? lastOperator = null;
            bool isUnary = true;
            // In the start of expression OR after ANY operator, next + or - is treated as unary

            foreach (var token in tokens)
            {
                if (token is OperatorToken opToken)
                {
                    lastOperator = opToken.OperationType;
                    isUnary = true; // next number may be unary
                }
                else if (token is NumericToken numToken)
                {
                    BigInteger value = numToken.NumericValue;

                    // handles unary ± (this applies ONLY when "isUnary == true")
                    if (isUnary)
                    {
                        if (lastOperator == OperationType.Subtraction)
                            value = -value;

                        // unary plus does nothing
                        isUnary = false;
                    }

                    // if lastOperator is null → it's the first number (or unary number)
                    if (lastOperator == null)
                    {
                        currentTerm = value;
                    }
                    else
                    {
                        switch (lastOperator)
                        {
                            case OperationType.Addition:
                                // resolve previous term
                                result += currentTerm;
                                currentTerm = value;
                                break;

                            case OperationType.Subtraction:
                                // subtraction acts like addition of negative term
                                result += currentTerm;
                                currentTerm = value;
                                break;

                            case OperationType.Multiplication:
                                currentTerm *= value;
                                break;

                            case OperationType.Division:
                                if (value == 0)
                                {
                                    return new ParsingResult
                                    {
                                        Success = false,
                                        ErrorMessage = "Invalid Expression: Division by zero is not allowed",
                                        ErrorType = ErrorTypeCore.Error
                                    };
                                }
                                currentTerm /= value;
                                break;
                        }
                    }

                    // operator was consumed
                    lastOperator = null;
                }
            }

            // Add last remaining term
            result += currentTerm;

            return new ParsingResult
            {
                Success = true,
                Value = result.ToString()
            };
        }
    }
}

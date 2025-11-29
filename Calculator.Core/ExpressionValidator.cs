using Calculator.Core.Interfaces;
using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core
{
    public class ExpressionValidator : IExpressionValidator
    {
        public ParsingResult ValidateExpression(string expression)
        {
            if(string.IsNullOrEmpty(expression))
            {
                return new ParsingResult
                {
                    Success = false,
                    ErrorMessage = "Input is empty",
                    ErrorType = Models.Enums.ErrorTypeCore.Warning
                };
            }
            //todo if contains dividation by zero etc..

            return new ParsingResult
            {
                Success = true
            };
        }
    }
}

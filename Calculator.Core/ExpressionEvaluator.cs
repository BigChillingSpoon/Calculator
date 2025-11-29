using Calculator.Core.Interfaces;
using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        private readonly IExpressionValidator _expressionValidator;
        public ExpressionEvaluator(IExpressionValidator expressionValidator)
        {
            _expressionValidator = expressionValidator;
        }

        public ParsingResult Evaluate(string expression)
        {
            try
            {
                var validationResult = _expressionValidator.ValidateExpression(expression);
                if (!validationResult.Success)
                    return validationResult;

                var computationResult = new System.Data.DataTable().Compute(expression, null);
                var value = Convert.ToDouble(computationResult);
                var result = new ParsingResult
                {
                    Success = true,
                    Value = value
                };
                return result;
            }
            catch (Exception ex)
            {
                return new ParsingResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ErrorType = Models.Enums.ErrorTypeCore.Error

                };
            }
        }
    }
}

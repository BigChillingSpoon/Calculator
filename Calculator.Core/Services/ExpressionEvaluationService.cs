
using Calculator.Core.Exceptions;
using Calculator.Core.Interfaces;
using Calculator.Core.Models;
using Calculator.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Services
{
    public class ExpressionEvaluationService : IExpressionEvaluationService
    {
        private readonly IExpressionValidator _expressionValidator;
        private readonly IExpressionTokenizer _expressionTokenizer;
        private readonly IExpressionNormalizer _expressionNormalizer;
        private readonly IExpressionEvaluator _expressionEvaluator;
        public ExpressionEvaluationService(IExpressionValidator expressionValidator, IExpressionTokenizer expressionTokenizer, IExpressionNormalizer expressionNormalizer, IExpressionEvaluator expressionEvaluator)
        {
            _expressionValidator = expressionValidator;
            _expressionTokenizer = expressionTokenizer;
            _expressionNormalizer = expressionNormalizer;
            _expressionEvaluator = expressionEvaluator;
        }

        public ParsingResult Evaluate(string expression)
        {
            //tokenize -> normalize -> validate -> compute
            try
            {
                // does only tokenization - if it finds unknown operator or unsupported character, throws exception
                var tokenizedExpression = _expressionTokenizer.Tokenize(expression);

                //normalizes sequences of operators (simple rules - e.g. -- -> +, +- -> - etc.) if it finds any invalid sequence it returns parsing result unsucesfull
                var normalizationResult = _expressionNormalizer.NormalizeExpression(tokenizedExpression);
                if(!normalizationResult.Success)
                    return normalizationResult;

                // validates expression syntax (e.g. no two operators in a row, expression doesn't start or end with an operator, etc.)
                var validationResult = _expressionValidator.ValidateExpression(tokenizedExpression);
                if (!validationResult.Success)
                    return validationResult;
                
                //evaluates expression
                var computationResult = _expressionEvaluator.EvaluateExpression(tokenizedExpression);
                return computationResult;
            }
            catch (UnsupportedCharacterException ex)
            {
                return new ParsingResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid expression: {ex.Message}",
                    ErrorType = ErrorTypeCore.Error
                };
            }
            catch(UnknownOperatorException ex)
            {
                return new ParsingResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid expression: {ex.Message}",
                    ErrorType = ErrorTypeCore.Error
                };
            }
        }
    }
}

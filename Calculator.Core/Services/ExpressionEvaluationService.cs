
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
            try
            {
                //tokenize -> normalize -> validate -> compute
                //provadi ciste tokenizaci - pokud najde neznamy operator nebo nepodporovany znak, vyhodi vyjimku
                var tokenizedExpression = _expressionTokenizer.Tokenize(expression);

                //normalizuje sekvence operatoru (jednoduche pravidla - napr. -- -> +, +- -> - atd.) pokud najde nejakou nevalidni sekvenci tak vrati parsing result unsucesfull
                //stara se o lexikalni spravnost vyrazu
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

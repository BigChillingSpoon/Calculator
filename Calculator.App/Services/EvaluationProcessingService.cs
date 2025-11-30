using Calculator.AppLayer.Models;
using Calculator.AppLayer.Interfaces;
using Calculator.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.AppLayer.Models.Enums;

namespace Calculator.AppLayer.Services
{
    public class EvaluationProcessingService : IEvaluationProcessingService
    {
        private readonly IExpressionEvaluationService _expressionEvaluator;
        public EvaluationProcessingService(IExpressionEvaluationService expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;
        }
        /// <summary>
        /// Processes evaluation of a single expression.
        /// All unexpected exceptions are handled in upper layers of application.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>ProcessResult based on parsing result values.</returns>
        public ProcessResult ProcessEvaluation(string expression)
        { 
            var parsingResult = _expressionEvaluator.Evaluate(expression);
            return new ProcessResult
            {
                Success = parsingResult.Success,
                Value = parsingResult.Value,
                ErrorMessage = parsingResult.ErrorMessage,
                ErrorType = (ErrorType)parsingResult.ErrorType
            };
        }
    }
}

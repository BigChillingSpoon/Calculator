using Calculator.AppLayer.Models;
using Calculator.AppLayer.Services.Interfaces;
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
        private readonly IExpressionEvaluator _expressionEvaluator;
        public EvaluationProcessingService(IExpressionEvaluator expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;
        }

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

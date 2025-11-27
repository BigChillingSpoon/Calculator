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
        public ParsingResult Evaluate(string expression)
        {
            try
            {
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
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}

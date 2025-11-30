using Calculator.Core.Interfaces;
using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Core.Models.Enums;
using Calculator.Core.Exceptions;
using System.Numerics;
using Calculator.Core.Models.Interfaces;
namespace Calculator.Core
{
    public class ExpressionTokenizer : IExpressionTokenizer
    {
        /// <summary>
        /// Tokenizes the input expression string into a list of expression tokens.
        /// Expression tokens are of type 
        /// <typeparamref name="IExpressionToken"/> (NumericToken, OperatorToken).
        /// </summary>
        /// <param name="expression">string expression</param>
        /// <returns>Tokenized expression</returns>
        /// <exception cref="UnsupportedCharacterException">/exception>
        public List<IExpressionToken> Tokenize(string expression)
        {
            List<IExpressionToken> tokens = new List<IExpressionToken>();
            StringBuilder currentNumber = new StringBuilder();
            foreach (char c in expression)
            {
                if (char.IsDigit(c))
                {
                    currentNumber.Append(c);
                }
                else
                {
                    //adds number token if exists
                    if (currentNumber.Length > 0)
                    {
                        tokens.Add(new NumericToken(currentNumber.ToString()));
                        currentNumber.Clear();
                    }
                    if(char.IsLetter(c))
                    {
                        throw new UnsupportedCharacterException();
                    }

                    //all blank spaces are ignored
                    if (!char.IsWhiteSpace(c))
                    {
                        //adds operator token
                        tokens.Add(new OperatorToken(c.ToString()));
                    }
                }
            }
            if (currentNumber.Length > 0)
            {
                tokens.Add(new NumericToken(currentNumber.ToString()));
            }
            return tokens;
        }
    }
}

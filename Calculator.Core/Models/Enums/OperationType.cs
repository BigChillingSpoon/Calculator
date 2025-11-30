using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Models.Enums
{
    public enum OperationType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Unknown
    }

    public static class OperationTypeExtensions
    {
        /// <summary>
        /// Converts OperationType enum to its corresponding symbol.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Operator type as string.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToSymbol(this OperationType type)
        {
            return type switch
            {
                OperationType.Addition => "+",
                OperationType.Subtraction => "-",
                OperationType.Multiplication => "*",
                OperationType.Division => "/",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.Core.Models;
using Calculator.Core.Models.Interfaces;

namespace Calculator.Tests.Helpers
{
    /// <summary>
    /// Helper methods for test assertions and setup
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Asserts that a result is successful
        /// </summary>
        public static void AssertSuccess(ParsingResult result, string message = "")
        {
            if (!result.Success)
            {
                throw new Xunit.Sdk.XunitException(
                    $"Expected success but got error: {result.ErrorMessage}. {message}");
            }
        }

        /// <summary>
        /// Asserts that a result failed
        /// </summary>
        public static void AssertFailure(ParsingResult result, string message = "")
        {
            if (result.Success)
            {
                throw new Xunit.Sdk.XunitException(
                    $"Expected failure but operation succeeded. {message}");
            }
        }

        /// <summary>
        /// Asserts result value equals expected
        /// </summary>
        public static void AssertValue(ParsingResult result, string expected)
        {
            AssertSuccess(result);
            if (result.Value != expected)
            {
                throw new Xunit.Sdk.XunitException(
                    $"Expected value '{expected}' but got '{result.Value}'");
            }
        }

        /// <summary>
        /// Asserts error message contains substring
        /// </summary>
        public static void AssertErrorContains(ParsingResult result, string substring)
        {
            AssertFailure(result);
            if (!result.ErrorMessage.Contains(substring, StringComparison.OrdinalIgnoreCase))
            {
                throw new Xunit.Sdk.XunitException(
                    $"Expected error to contain '{substring}' but got '{result.ErrorMessage}'");
            }
        }

        /// <summary>
        /// Creates a numeric token
        /// </summary>
        public static NumericToken Number(string value) => new NumericToken(value);

        /// <summary>
        /// Creates an operator token
        /// </summary>
        public static OperatorToken Op(string op) => new OperatorToken(op);

        /// <summary>
        /// Verifies token list matches expected types and values
        /// </summary>
        public static void AssertTokens(List<IExpressionToken> actual, params IExpressionToken[] expected)
        {
            if (actual.Count != expected.Length)
            {
                throw new Xunit.Sdk.XunitException(
                    $"Expected {expected.Length} tokens but got {actual.Count}");
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (actual[i].GetType() != expected[i].GetType())
                {
                    throw new Xunit.Sdk.XunitException(
                        $"Token {i}: Expected type {expected[i].GetType().Name} but got {actual[i].GetType().Name}");
                }

                if (actual[i].RawValue != expected[i].RawValue)
                {
                    throw new Xunit.Sdk.XunitException(
                        $"Token {i}: Expected value '{expected[i].RawValue}' but got '{actual[i].RawValue}'");
                }
            }
        }
    }

    /// <summary>
    /// Test data generator for theory tests
    /// </summary>
    public static class TestData
    {
        public static IEnumerable<object[]> BasicArithmeticExpressions()
        {
            yield return new object[] { "1+1", "2" };
            yield return new object[] { "5-3", "2" };
            yield return new object[] { "4*3", "12" };
            yield return new object[] { "10/2", "5" };
            yield return new object[] { "0+0", "0" };
            yield return new object[] { "100-100", "0" };
        }

        public static IEnumerable<object[]> PrecedenceExpressions()
        {
            yield return new object[] { "2+3*4", "14" };
            yield return new object[] { "10-6/2", "7" };
            yield return new object[] { "2*3+4*5", "26" };
            yield return new object[] { "100/10-5", "5" };
        }

        public static IEnumerable<object[]> NegativeNumberExpressions()
        {
            yield return new object[] { "-5", "-5" };
            yield return new object[] { "-10+5", "-5" };
            yield return new object[] { "10+-5", "5" };
            yield return new object[] { "-3*-2", "6" };
            yield return new object[] { "2+-3*2", "-4" };
        }

        public static IEnumerable<object[]> InvalidExpressions()
        {
            yield return new object[] { "2.5", "operator" };
            yield return new object[] { "a+1", "character" };
            yield return new object[] { "2+", "operator" };
            yield return new object[] { "**2", "sequence" };
            yield return new object[] { "", "empty" };
        }

        public static IEnumerable<object[]> LargeNumbers()
        {
            yield return new object[] { "999999999999999999999999999999", "999999999999999999999999999999" };
            yield return new object[] { "123456789012345678901234567890+1", "123456789012345678901234567891" };
            yield return new object[] { "10000000000000000000*2", "20000000000000000000" };
        }

        public static IEnumerable<object[]> DivisionByZero()
        {
            yield return new object[] { "5/0" };
            yield return new object[] { "10+5/0-3" };
            yield return new object[] { "0/0" };
        }

        public static IEnumerable<object[]> WhitespaceExpressions()
        {
            yield return new object[] { " 3 + 4 ", "7" };
            yield return new object[] { "10 -  2* 3", "4" };
            yield return new object[] { "  5/                1 ", "5" };
        }
    }
}
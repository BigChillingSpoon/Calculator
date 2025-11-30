using System.Collections.Generic;
using System.Numerics;
using Calculator.Core;
using Calculator.Core.Models;
using Calculator.Core.Models.Enums;
using Calculator.Core.Models.Interfaces;
using Calculator.Tests.Helpers;
using Xunit;

namespace Calculator.Core.Tests
{
    public class EvaluatorTests
    {
        private readonly ExpressionEvaluator _evaluator;

        public EvaluatorTests()
        {
            _evaluator = new ExpressionEvaluator();
        }

        #region Basic Operations

        [Theory]
        [InlineData("2", "3", "5")]
        [InlineData("10", "5", "15")]
        [InlineData("0", "0", "0")]
        [InlineData("100", "200", "300")]
        public void Evaluate_Addition_ReturnsCorrectSum(string a, string b, string expected)
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken(a),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken(b)
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        [Theory]
        [InlineData("5", "3", "2")]
        [InlineData("10", "10", "0")]
        [InlineData("100", "1", "99")]
        public void Evaluate_Subtraction_ReturnsCorrectDifference(string a, string b, string expected)
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken(a),
                new BinaryOperatorToken(OperationType.Subtraction),
                new NumericToken(b)
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        [Theory]
        [InlineData("2", "3", "6")]
        [InlineData("5", "5", "25")]
        [InlineData("10", "0", "0")]
        [InlineData("7", "8", "56")]
        public void Evaluate_Multiplication_ReturnsCorrectProduct(string a, string b, string expected)
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken(a),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken(b)
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        [Theory]
        [InlineData("10", "2", "5")]
        [InlineData("100", "10", "10")]
        [InlineData("7", "2", "3")] // Integer division
        [InlineData("0", "5", "0")]
        public void Evaluate_Division_ReturnsCorrectQuotient(string a, string b, string expected)
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken(a),
                new BinaryOperatorToken(OperationType.Division),
                new NumericToken(b)
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        #endregion

        #region Operator Precedence

        [Fact]
        public void Evaluate_MultiplicationBeforeAddition_RespectsPrecedence()
        {
            // Arrange - "2 + 3 * 4" = 14, not 20
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("3"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("4")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "14");
        }

        [Fact]
        public void Evaluate_DivisionBeforeSubtraction_RespectsPrecdence()
        {
            // Arrange - "10 - 6 / 2" = 7, not 2
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("10"),
                new BinaryOperatorToken(OperationType.Subtraction),
                new NumericToken("6"),
                new BinaryOperatorToken(OperationType.Division),
                new NumericToken("2")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "7");
        }

        [Fact]
        public void Evaluate_MultipleMultiplications_LeftToRight()
        {
            // Arrange - "2 * 3 * 4" = 24
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("3"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("4")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "24");
        }

        [Fact]
        public void Evaluate_ComplexPrecedence_Correct()
        {
            // Arrange - "2 + 3 * 4 - 10 / 2" = 2 + 12 - 5 = 9
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("3"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("4"),
                new BinaryOperatorToken(OperationType.Subtraction),
                new NumericToken("10"),
                new BinaryOperatorToken(OperationType.Division),
                new NumericToken("2")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "9");
        }

        #endregion

        #region Negative Numbers

        [Fact]
        public void Evaluate_SingleNegativeNumber_ReturnsNegative()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("-5")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "-5");
        }

        [Fact]
        public void Evaluate_AdditionWithNegative_Correct()
        {
            // Arrange - "10 + (-5)" = 5
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("10"),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("-5")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "5");
        }

        [Fact]
        public void Evaluate_NegativeMultiplication_Correct()
        {
            // Arrange - "(-3) * 2" = -6
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("-3"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("2")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "-6");
        }

        [Fact]
        public void Evaluate_TwoNegativesMultiplied_BecomesPositive()
        {
            // Arrange - "(-3) * (-2)" = 6
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("-3"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("-2")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "6");
        }

        #endregion

        #region Division by Zero

        [Fact]// 5/0 is not valid
        public void Evaluate_DivisionByZero_ReturnsError()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("5"),
                new BinaryOperatorToken(OperationType.Division),
                new NumericToken("0")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "division by zero");
        }

        [Fact] // 0/0 case
        public void Evaluate_ZeroDividedByZero_ReturnsError()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("0"),
                new BinaryOperatorToken(OperationType.Division),
                new NumericToken("0")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
        }

        [Fact]
        public void Evaluate_DivisionByZeroInMiddle_ReturnsError()
        {
            // Arrange - "10 + 5 / 0 - 3" -> means error due to division by zero
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("10"),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("5"),
                new BinaryOperatorToken(OperationType.Division),
                new NumericToken("0"),
                new BinaryOperatorToken(OperationType.Subtraction),
                new NumericToken("3")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
        }

        #endregion

        #region Large Numbers

        [Fact]//large adition
        public void Evaluate_LargeNumberAddition_Correct()
        {
            // Arrange
            var large1 = "999999999999999999999999999999";
            var large2 = "1";
            var expected = "1000000000000000000000000000000";

            var tokens = new List<IExpressionToken>
            {
                new NumericToken(large1),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken(large2)
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        [Fact]//multiplication test for large numbers
        public void Evaluate_LargeNumberMultiplication_Correct()
        {
            // Arrange
            var large = "10000000000000000000";
            var tokens = new List<IExpressionToken>
            {
                new NumericToken(large),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("2")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "20000000000000000000");
        }

        #endregion

        #region Edge Cases

        [Fact]// empty is empty
        public void Evaluate_EmptyList_ReturnsError()
        {
            // Arrange
            var tokens = new List<IExpressionToken>();

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
            Assert.Equal(ErrorTypeCore.Error, result.ErrorType);
        }

        [Fact]// 0 is 0
        public void Evaluate_SingleZero_ReturnsZero()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("0")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "0");
        }

        [Fact]// tests wrong recongition of division by zero error - 0 + 0 * 0 - 0 = 0 is valid
        public void Evaluate_MultipleZeros_ReturnsZero()
        {
            // Arrange - "0 + 0 * 0 - 0"
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("0"),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("0"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("0"),
                new BinaryOperatorToken(OperationType.Subtraction),
                new NumericToken("0")
            };

            // Act
            var result = _evaluator.EvaluateExpression(tokens);

            // Assert
            TestHelpers.AssertValue(result, "0");
        }

        #endregion
    }
}
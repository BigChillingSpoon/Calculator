using System.Collections.Generic;
using Calculator.Core;
using Calculator.Core.Models;
using Calculator.Core.Models.Enums;
using Calculator.Core.Models.Interfaces;
using Calculator.Tests.Helpers;
using Xunit;

namespace Calculator.Core.Tests
{
    public class ValidatorTests
    {
        private readonly ExpressionValidator _validator;

        public ValidatorTests()
        {
            _validator = new ExpressionValidator();
        }

        #region Valid Expressions

        [Fact]// 2+3 is valid
        public void Validate_SimpleAddition_Passes()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("3")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
        }

        [Fact]// 42 is valid
        public void Validate_SingleNumber_Passes()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("42")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
        }

        [Fact]// 2+3*4 is valid
        public void Validate_ComplexExpression_Passes()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("3"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("4")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
        }

        [Fact]// -5 is valid
        public void Validate_NegativeNumber_Passes()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("-5")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
        }

        #endregion

        #region Empty or Null

        [Fact]// empty list should fail - with WARNING
        public void Validate_EmptyList_Fails()
        {
            // Arrange
            var tokens = new List<IExpressionToken>();

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "empty");// error message will always contain information about emptiness
            Assert.Equal(ErrorTypeCore.Warning, result.ErrorType);
        }

        [Fact]// null means empty expression
        public void Validate_NullList_Fails()
        {
            // Arrange
            List<IExpressionToken>? tokens = null;

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "empty");// error message will always contain information about emptiness
            Assert.Equal(ErrorTypeCore.Warning, result.ErrorType);
        }

        #endregion

        #region Ends with Operator

        [Fact]// expression cannot end with an operator
        public void Validate_EndsWithPlus_Fails()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Addition)
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "end");
            Assert.Equal(ErrorTypeCore.Error, result.ErrorType);
        }

        [Fact]// expression cannot end with an operator
        public void Validate_EndsWithMultiplication_Fails()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("5"),
                new BinaryOperatorToken(OperationType.Multiplication)
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "operator");
        }

        #endregion

        #region Operator Sequences
        [Fact] // no sequence of operators is allowed
        public void Validate_TwoSignOperatorsInRow_Fails()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Addition),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("3")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
        }

        [Fact] // no sequence of operators is allowed
        public void Validate_TwoMultiplicationInRow_Fails()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("3")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "sequence");
        }

        [Fact]// division is not sign operator
        public void Validate_MultiplicationFollowedByDivision_Fails()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new BinaryOperatorToken(OperationType.Division),
                new NumericToken("3")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "invalid sequence");
        }

        [Fact]// +* is invalid
        public void Validate_AdditionFollowedByMultiplication_Fails()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Addition),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("3")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertFailure(result);
        }

        #endregion

        #region Edge Cases

        [Fact]//long sequences are valid
        public void Validate_VeryLongExpression_Passes()
        {
            // Arrange
            var tokens = new List<IExpressionToken>();
            for (int i = 0; i < 100; i++)
            {
                tokens.Add(new NumericToken(i.ToString()));
                if (i < 99)
                    tokens.Add(new BinaryOperatorToken(OperationType.Addition));
            }

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
        }

        [Fact]//1+2-3*4/5 is valid
        public void Validate_AlternatingOperations_Passes()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("1"),
                new BinaryOperatorToken(OperationType.Addition),
                new NumericToken("2"),
                new BinaryOperatorToken(OperationType.Subtraction),
                new NumericToken("3"),
                new BinaryOperatorToken(OperationType.Multiplication),
                new NumericToken("4"),
                new BinaryOperatorToken(OperationType.Division),
                new NumericToken("5")
            };

            // Act
            var result = _validator.ValidateExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
        }

        #endregion
    }
}
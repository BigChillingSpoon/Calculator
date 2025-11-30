using Calculator.Core;
using Calculator.Core.Models.Interfaces;
using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Tests.Helpers;


namespace Calculator.Tests.Core.Normalization
{
    public class UnaryClassifierTests
    {
        private readonly UnaryClassifier _unaryClassifier;
        public UnaryClassifierTests()
        {
            _unaryClassifier = new UnaryClassifier();
        }
        [Fact]
        public void UnaryClassifier_MinusAtStart_MarksAsUnary()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new OperatorToken("-"),
                new NumericToken("5")
            };

            // Act
            var result = _unaryClassifier.ClassifyUnaryOperators(tokens);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.IsType<UnaryOperatorToken>(result[0]);
            Assert.Equal("-", result[0].RawValue);
        }

        [Fact]
        public void UnaryClassifier_MinusAfterMultiplication_MarksAsUnary()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new OperatorToken("*"),
                new OperatorToken("-"),
                new NumericToken("3")
            };

            // Act
            var result = _unaryClassifier.ClassifyUnaryOperators(tokens);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.IsType<BinaryOperatorToken>(result[1]); // *
            Assert.IsType<UnaryOperatorToken>(result[2]);  // -
        }

        [Fact]
        public void UnaryClassifier_MinusBetweenNumbers_MarksAsBinary()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("5"),
                new OperatorToken("-"),
                new NumericToken("3")
            };

            // Act
            var result = _unaryClassifier.ClassifyUnaryOperators(tokens);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.IsType<BinaryOperatorToken>(result[1]);
            Assert.Equal("-", result[1].RawValue);
        }

        [Fact]
        public void UnaryClassifier_PlusAtStart_MarksAsUnary()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new OperatorToken("+"),
                new NumericToken("5")
            };

            // Act
            var result = _unaryClassifier.ClassifyUnaryOperators(tokens);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.IsType<UnaryOperatorToken>(result[0]);
        }
    }
}

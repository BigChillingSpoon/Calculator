using Calculator.Core;
using Calculator.Core.Models.Interfaces;
using Calculator.Core.Models;
using Calculator.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Tests.Core.Normalization
{
    public class NormalizerPipelineTests
    {
        private readonly ExpressionNormalizer _normalizer;
        private readonly SignNormalizer _signNormalizer;
        private readonly UnaryClassifier _unaryClassifier;
        private readonly UnaryMerger _unaryMerger; 

        public NormalizerPipelineTests()
        {
            _signNormalizer = new SignNormalizer();
            _unaryClassifier = new UnaryClassifier();
            _unaryMerger = new UnaryMerger();
            _normalizer = new ExpressionNormalizer(_unaryClassifier, _signNormalizer, _unaryMerger);
        }

        [Fact]
        public void Normalizer_CompleteNormalization_WorksCorrectly()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new OperatorToken("+"),
                new OperatorToken("-"),
                new NumericToken("3")
            };

            // Act
            var result = _normalizer.NormalizeExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
            Assert.Equal(3, tokens.Count); // Modified in place
            Assert.IsType<NumericToken>(tokens[0]);
            Assert.IsType<BinaryOperatorToken>(tokens[1]);//binary - , + is ignored in sign normalizer
            Assert.IsType<NumericToken>(tokens[2]);

            var lastNum = (NumericToken)tokens[2];
            Assert.Equal(new BigInteger(3), lastNum.NumericValue);
        }

        [Theory]
        [InlineData("2", "+", "-", "3")]// + is binary - is unary
        [InlineData("10", "*", "-", "5")]// * is binary - is unary
        [InlineData("8", "/", "-", "2")]// / is binary - is unary
        public void Normalizer_BinaryFollowedByUnary_NormalizesCorrectly(
            string num1, string op, string sign, string num2)
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken(num1),
                new OperatorToken(op),
                new OperatorToken(sign),
                new NumericToken(num2)
            };

            // Act
            var result = _normalizer.NormalizeExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
            Assert.Equal(3, tokens.Count);
        }

        [Fact]
        public void Normalizer_MultipleConsecutiveSigns_NormalizesAll()
        {
            // Arrange - "2 + -- 3" should become "2 + 3"
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new OperatorToken("+"),
                new OperatorToken("-"),
                new OperatorToken("-"),
                new NumericToken("3")
            };

            // Act
            var result = _normalizer.NormalizeExpression(tokens);

            // Assert
            TestHelpers.AssertSuccess(result);
            Assert.Equal(3, tokens.Count);
            var lastNum = (NumericToken)tokens[2];
            Assert.Equal(new BigInteger(3), lastNum.NumericValue); // Double negative = positive
        }

    }
}

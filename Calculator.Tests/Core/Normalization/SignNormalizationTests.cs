using Calculator.Core;
using Calculator.Core.Models.Interfaces;
using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Tests.Core.Normalization
{
    public class SignNormalizationTests
    {
        private readonly SignNormalizer _signNormalizer;

        public SignNormalizationTests()
        {
            _signNormalizer = new SignNormalizer();
        }
        [Fact]// -- = +
        public void SignNormalizer_DoubleNegative_BecomesPositive()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new OperatorToken("-"),
                new OperatorToken("-"),
                new NumericToken("3")
            };

            // Act
            var result = _signNormalizer.NormalizeSigns(tokens);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("2", result[0].RawValue);
            Assert.Equal("+", result[1].RawValue); // -- becomes +
            Assert.Equal("3", result[2].RawValue);
        }

        [Fact] //+- = -
        public void SignNormalizer_PlusMinus_BecomesMinus()
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
            var result = _signNormalizer.NormalizeSigns(tokens);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("-", result[1].RawValue); // +- becomes -
        }

        [Fact]// --- = -
        public void SignNormalizer_TripleNegative_BecomesMinus()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new NumericToken("2"),
                new OperatorToken("-"),
                new OperatorToken("-"),
                new OperatorToken("-"),
                new NumericToken("3")
            };

            // Act
            var result = _signNormalizer.NormalizeSigns(tokens);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("-", result[1].RawValue);
        }

        [Fact]//no normalization needed
        public void SignNormalizer_MultiplicationFollowedByMinus_PreservesOperators()
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
            var result = _signNormalizer.NormalizeSigns(tokens);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal("*", result[1].RawValue);
            Assert.Equal("-", result[2].RawValue); // Unary minus preserved
        }
    }
}

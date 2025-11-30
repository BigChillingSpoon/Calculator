using Calculator.Core.Models.Enums;
using Calculator.Core.Models.Interfaces;
using Calculator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Calculator.Core;

namespace Calculator.Tests.Core.Normalization
{
    public class UnaryMergerTests 
    {
        private readonly UnaryMerger _unaryMerger;
        public UnaryMergerTests()
        {
            _unaryMerger = new UnaryMerger();
        }
        [Fact] //[-,5] shall be merged into NumericToken -5
        public void UnaryMerger_UnaryMinus_MergesWithNumber()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new UnaryOperatorToken(OperationType.Subtraction),
                new NumericToken("5")
            };

            // Act
            var result = _unaryMerger.MergeUnaryOperators(tokens);

            // Assert
            Assert.Single(result);
            var numToken = Assert.IsType<NumericToken>(result[0]);
            Assert.Equal(new BigInteger(-5), numToken.NumericValue);
        }

        [Fact]// [+,5] shall be merged into NumericToken 5 - ignore addition
        public void UnaryMerger_UnaryPlus_RemovesSign()
        {
            // Arrange
            var tokens = new List<IExpressionToken>
            {
                new UnaryOperatorToken(OperationType.Addition),
                new NumericToken("5")
            };

            // Act
            var result = _unaryMerger.MergeUnaryOperators(tokens);

            // Assert
            Assert.Single(result);
            var numToken = Assert.IsType<NumericToken>(result[0]);
            Assert.Equal(new BigInteger(5), numToken.NumericValue);
        }

        [Fact]// test with multiple unary operators
        public void UnaryMerger_MultipleUnaryOperators_MergesAll()
        {
            // Arrange -> represents -5+-3
            var tokens = new List<IExpressionToken>
            {
                new UnaryOperatorToken(OperationType.Subtraction),
                new NumericToken("5"),
                new BinaryOperatorToken(OperationType.Addition),
                new UnaryOperatorToken(OperationType.Subtraction),
                new NumericToken("3")
            };

            // Act
            var result = _unaryMerger.MergeUnaryOperators(tokens);

            // Assert
            Assert.Equal(3, result.Count);
            var first = Assert.IsType<NumericToken>(result[0]);
            Assert.Equal(new BigInteger(-5), first.NumericValue);
            var third = Assert.IsType<NumericToken>(result[2]);
            Assert.Equal(new BigInteger(-3), third.NumericValue);
        }

    }
}

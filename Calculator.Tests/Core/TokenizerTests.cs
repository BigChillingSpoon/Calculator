using System;
using System.Collections.Generic;
using Calculator.Core;
using Calculator.Core.Exceptions;
using Calculator.Core.Models;
using Calculator.Tests.Helpers;
using Xunit;

namespace Calculator.Core.Tests
{
    public class TokenizerTests
    {
        private readonly ExpressionTokenizer _tokenizer;

        public TokenizerTests()
        {
            _tokenizer = new ExpressionTokenizer();
        }

        #region Basic Tokenization

        [Fact]//token per character
        public void Tokenize_SimpleAddition_ReturnsThreeTokens()
        {
            // Arrange
            var expression = "2+3";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(3, tokens.Count);
            TestHelpers.AssertTokens(tokens,
                TestHelpers.Number("2"),
                TestHelpers.Op("+"),
                TestHelpers.Number("3"));
        }

        [Fact]//tokens per Token type (number is sequence of digits)
        public void Tokenize_MultipleDigitNumber_ParsesCorrectly()
        {
            // Arrange
            var expression = "123+456";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(3, tokens.Count);
            Assert.Equal("123", tokens[0].RawValue);
            Assert.Equal("456", tokens[2].RawValue);
        }

        [Fact]//count of chars = count of tokens
        public void Tokenize_AllOperators_RecognizesAll()
        {
            // Arrange
            var expression = "1+2-3*4/5";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(9, tokens.Count);
            Assert.IsType<NumericToken>(tokens[0]);
            Assert.IsType<OperatorToken>(tokens[1]);
            Assert.Equal("+", tokens[1].RawValue);
            Assert.Equal("-", tokens[3].RawValue);
            Assert.Equal("*", tokens[5].RawValue);
            Assert.Equal("/", tokens[7].RawValue);
        }

        [Fact]// ignores whitespaces
        public void Tokenize_WithWhitespace_IgnoresSpaces()
        {
            // Arrange
            var expression = "  2  +  3  ";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(3, tokens.Count);
            TestHelpers.AssertTokens(tokens,
                TestHelpers.Number("2"),
                TestHelpers.Op("+"),
                TestHelpers.Number("3"));
        }

        [Fact]//each character is token
        public void Tokenize_ConsecutiveOperators_TokenizesAll()
        {
            // Arrange
            var expression = "2+-3";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(4, tokens.Count);
            Assert.Equal("+", tokens[1].RawValue);
            Assert.Equal("-", tokens[2].RawValue);
        }

        #endregion

        #region Invalid Characters

        [Theory]//expression with letters should be handled UnsupportedCharacterException
        [InlineData("a")]
        [InlineData("2+a")]
        [InlineData("abc")]
        [InlineData("2+3x")]
        public void Tokenize_ContainsLetter_ThrowsException(string expression)
        {
            // Act & Assert
            Assert.Throws<UnsupportedCharacterException>(() =>
                _tokenizer.Tokenize(expression));
        }

        [Theory]// invalid operators are non letters characters that are not supported as operators(see enum OperationType.cs)
        [InlineData("2%3")]
        [InlineData("2^3")]
        [InlineData("2&3")]
        [InlineData("2|3")]
        public void Tokenize_InvalidOperator_ThrowsException(string expression)
        {
            // Act & Assert
            Assert.Throws<UnknownOperatorException>(() =>
                _tokenizer.Tokenize(expression));
        }

        #endregion

        #region Edge Cases

        [Fact]// empty string = 0 tokens
        public void Tokenize_EmptyString_ReturnsEmptyList()
        {
            // Arrange
            var expression = "";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Empty(tokens);
        }

        [Fact] //all whitesaces must be ignored during tokenization
        public void Tokenize_OnlyWhitespace_ReturnsEmptyList()
        {
            // Arrange
            var expression = "   ";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Empty(tokens);
        }

        [Fact] //single number without any operator before should be tokenized into one token of type NumericToken
        public void Tokenize_SingleNumber_ReturnsSingleToken()
        {
            // Arrange
            var expression = "42";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Single(tokens);
            Assert.IsType<NumericToken>(tokens[0]);
            Assert.Equal("42", tokens[0].RawValue);
        }

        [Fact]// same as previous but for large numbers
        public void Tokenize_LargeNumber_ParsesCorrectly()
        {
            // Arrange
            var expression = "999999999999999999999999999999";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Single(tokens);
            Assert.Equal(expression, tokens[0].RawValue);
        }

        [Fact]// count of tokens should be 3, all blank spaces shall be ignored
        public void Tokenize_MultipleConsecutiveSpaces_IgnoresAll()
        {
            // Arrange
            var expression = "2     +     3";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(3, tokens.Count);
        }

        #endregion

        #region Complex Expressions

        [Fact] //ignore whitespaces + corectly tokenize
        public void Tokenize_ComplexExpression_TokenizesCorrectly()
        {
            // Arrange
            var expression = "10 + 20 * 3 - 5 / 5";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(9, tokens.Count);
            Assert.Equal("10", tokens[0].RawValue);
            Assert.Equal("+", tokens[1].RawValue);
            Assert.Equal("20", tokens[2].RawValue);
            Assert.Equal("*", tokens[3].RawValue);
            Assert.Equal("3", tokens[4].RawValue);
            Assert.Equal("-", tokens[5].RawValue);
            Assert.Equal("5", tokens[6].RawValue);
            Assert.Equal("/", tokens[7].RawValue);
            Assert.Equal("5", tokens[8].RawValue);
        }

        [Fact]// negative numbers must be tokenized into two tokens OperatorToken and NumericToken
        public void Tokenize_NegativeNumberAtStart_TokenizesMinusSign()
        {
            // Arrange
            var expression = "-5";

            // Act
            var tokens = _tokenizer.Tokenize(expression);

            // Assert
            Assert.Equal(2, tokens.Count);
            Assert.IsType<OperatorToken>(tokens[0]);
            Assert.Equal("-", tokens[0].RawValue);
            Assert.IsType<NumericToken>(tokens[1]);
            Assert.Equal("5", tokens[1].RawValue);
        }

        #endregion
    }
}
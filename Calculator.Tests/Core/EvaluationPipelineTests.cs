using Calculator.Core;
using Calculator.Core.Interfaces;
using Calculator.Core.Services;
using Calculator.Tests.Helpers;
using Xunit;

namespace Calculator.Core.Tests.Integration
{
    /// <summary>
    /// Integration tests for the complete expression evaluation pipeline
    /// Tests the full flow: Input → Tokenize → Normalize → Validate → Evaluate → Output
    /// </summary>
    public class EvaliationPipelineTests
    {
        private readonly IExpressionEvaluationService _service;

        public EvaliationPipelineTests()
        {
            // Setup complete pipeline
            var tokenizer = new ExpressionTokenizer();
            var validator = new ExpressionValidator();
            var evaluator = new ExpressionEvaluator();

            var signNormalizer = new SignNormalizer();
            var unaryClassifier = new UnaryClassifier();
            var unaryMerger = new UnaryMerger();
            var normalizer = new ExpressionNormalizer(unaryClassifier, signNormalizer, unaryMerger);

            _service = new ExpressionEvaluationService(validator, tokenizer, normalizer, evaluator);
        }

        #region Basic Arithmetic (from TestData)

        [Theory]
        [MemberData(nameof(TestData.BasicArithmeticExpressions), MemberType = typeof(TestData))]
        public void Evaluate_BasicArithmetic_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        [Theory]
        [MemberData(nameof(TestData.PrecedenceExpressions), MemberType = typeof(TestData))]
        public void Evaluate_WithPrecedence_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        [Theory]
        [MemberData(nameof(TestData.NegativeNumberExpressions), MemberType = typeof(TestData))]
        public void Evaluate_WithNegatives_ReturnsCorrectResult(string expression, string expected)
        {
            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        #endregion

        #region Invalid Expressions

        [Theory]
        [MemberData(nameof(TestData.InvalidExpressions), MemberType = typeof(TestData))]
        public void Evaluate_InvalidExpression_ReturnsError(string expression, string errorSubstring)
        {
            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, errorSubstring);
        }

        [Theory]
        [MemberData(nameof(TestData.DivisionByZero), MemberType = typeof(TestData))]
        public void Evaluate_DivisionByZero_ReturnsError(string expression)
        {
            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "division by zero");
        }

        #endregion

        #region Large Numbers

        [Theory]
        [MemberData(nameof(TestData.LargeNumbers), MemberType = typeof(TestData))]
        public void Evaluate_LargeNumbers_HandlesCorrectly(string expression, string expected)
        {
            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        #endregion

        #region Real-World Scenarios

        [Fact]
        public void Evaluate_ExampleFromRequirements_2Plus3Times2_Returns8()
        {
            // This is the example from requirements: 2 + 3 * 2 = 8

            // Act
            var result = _service.Evaluate("2 + 3 * 2");

            // Assert
            TestHelpers.AssertValue(result, "8");
        }

        [Fact]
        public void Evaluate_ExampleFromRequirements_2PlusMinus3Times2_ReturnsMinus4()
        {
            // From example input file: 2 + -3 * 2 = -4

            // Act
            var result = _service.Evaluate("2 + -3 * 2");

            // Assert
            TestHelpers.AssertValue(result, "-4");
        }

        [Fact]
        public void Evaluate_ExampleFromRequirements_2Divide3_Returns0()
        {
            // From example: 2 / 3 = 0 (integer division)

            // Act
            var result = _service.Evaluate("2 / 3");

            // Assert
            TestHelpers.AssertValue(result, "0");
        }

        [Fact]
        public void Evaluate_ExampleFromRequirements_InvalidCharacterA_ReturnsError()
        {
            // From example: a + 1 → Error - Invalid character: 'a'

            // Act
            var result = _service.Evaluate("a + 1");

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "character");
        }

        [Fact]
        public void Evaluate_ExampleFromRequirements_DecimalNumber_ReturnsError()
        {
            // From example: 2.1*2 → Error - Invalid character: '.'

            // Act
            var result = _service.Evaluate("2.1*2");

            // Assert
            TestHelpers.AssertFailure(result);
            TestHelpers.AssertErrorContains(result, "operator");
        }

        #endregion

        #region Complex Expressions

        [Fact]
        public void Evaluate_LongExpression_CalculatesCorrectly()
        {
            // Arrange - "1 + 2 * 3 - 4 / 2 + 5"
            // = 1 + 6 - 2 + 5 = 10
            var expression = "1 + 2 * 3 - 4 / 2 + 5";

            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, "10");
        }

        [Fact]
        public void Evaluate_ManyNegatives_HandlesCorrectly()
        {
            // Arrange - "-5 + -3 - -2"
            // = -5 + (-3) - (-2) = -5 - 3 + 2 = -6
            var expression = "-5 + -3 - -2";

            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, "-6");
        }

        [Fact]
        public void Evaluate_AllOperators_WorksTogether()
        {
            // Arrange - "10 + 5 - 3 * 2 / 2"
            // = 10 + 5 - 3 = 12
            var expression = "10 + 5 - 3 * 2 / 2";

            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, "12");
        }

        #endregion

        #region Whitespace Handling

        [Theory]
        [MemberData(nameof(TestData.WhitespaceExpressions), MemberType = typeof(TestData))]
        public void Evaluate_VariousWhitespace_ProducesSameResult(string expression, string expected)
        {
            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, expected);
        }

        #endregion

        #region Stress Tests

        [Fact]
        public void Evaluate_VeryLongExpression_Completes()
        {
            // Arrange - "1 + 1 + 1 + ... (100 times)"
            var parts = new string[199]; // 100 numbers, 99 operators
            for (int i = 0; i < 100; i++)
            {
                parts[i * 2] = "1";
                if (i < 99)
                    parts[i * 2 + 1] = "+";
            }
            var expression = string.Join(" ", parts);

            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertValue(result, "100");
        }

        [Fact]
        public void Evaluate_MassiveNumber_Handles()
        {
            // Arrange - Number with 100 digits
            var largeNum = new string('9', 100);
            var expression = $"{largeNum} + 1";

            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertSuccess(result);
            Assert.NotEmpty(result.Value);
            Assert.StartsWith("1", result.Value);
        }

        #endregion

        #region Edge Cases

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\n")]
        public void Evaluate_EmptyOrWhitespace_ReturnsError(string expression)
        {
            // Act
            var result = _service.Evaluate(expression);

            // Assert
            TestHelpers.AssertFailure(result);
        }

        [Fact] // 0 = 0
        public void Evaluate_JustZero_ReturnsZero()
        {
            // Act
            var result = _service.Evaluate("0");

            // Assert
            TestHelpers.AssertValue(result, "0");
        }

        [Fact] // -0 is 0
        public void Evaluate_NegativeZero_ReturnsZero()
        {
            // Act
            var result = _service.Evaluate("-0");

            // Assert
            TestHelpers.AssertValue(result, "0");
        }

        #endregion
    }
}
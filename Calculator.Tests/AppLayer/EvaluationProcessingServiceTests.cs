using Calculator.AppLayer.Services;
using Calculator.Core.Interfaces;
using Xunit;

/// <summary>
/// Unit tests for EvaluationProcessingService
/// </summary>
namespace Calculator.Tests.AppLayer;
public class EvaluationProcessingServiceTests
{
    private class FakeEvaluator : IExpressionEvaluationService
    {
        public Calculator.Core.Models.ParsingResult ResultToReturn { get; set; }

        public Calculator.Core.Models.ParsingResult Evaluate(string expression)
            => ResultToReturn;
    }

    private readonly FakeEvaluator _fakeEvaluator;
    private readonly EvaluationProcessingService _service;

    public EvaluationProcessingServiceTests()
    {
        _fakeEvaluator = new FakeEvaluator();
        _service = new EvaluationProcessingService(_fakeEvaluator);
    }

    [Fact]// tests simple addition
    public void ProcessEvaluation_ReturnsSuccess()
    {
        // Arrange
        _fakeEvaluator.ResultToReturn = new Calculator.Core.Models.ParsingResult
        {
            Success = true,
            Value = "10",
            ErrorMessage = string.Empty,
            ErrorType = Calculator.Core.Models.Enums.ErrorTypeCore.None
        };

        // Act
        var result = _service.ProcessEvaluation("5+5");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(_fakeEvaluator.ResultToReturn.Value, result.Value);
    }

    [Fact]// tests integer division with negative signs, integer division rounding towards zero
    public void ProcessEvaluation_ReturnsSuccess_ValueZero()
    {
        // Arrange
        _fakeEvaluator.ResultToReturn = new Calculator.Core.Models.ParsingResult
        {
            Success = true,
            Value = "0",
            ErrorMessage = string.Empty,
            ErrorType = Calculator.Core.Models.Enums.ErrorTypeCore.None
        };

        // Act
        var result = _service.ProcessEvaluation("-2/--9");

        // Assert
        Assert.True(result.Success);
        Assert.Equal(_fakeEvaluator.ResultToReturn.Value, result.Value);
    }

    [Fact]
    public void ProcessEvaluation_ReturnsFailure()
    {
        // Arrange
        _fakeEvaluator.ResultToReturn = new Calculator.Core.Models.ParsingResult
        {
            Success = false,
            Value = string.Empty,
            ErrorMessage = "Invalid expression",
            ErrorType = Calculator.Core.Models.Enums.ErrorTypeCore.Error
        };

        // Act
        var result = _service.ProcessEvaluation("1*a");

        // Assert
        Assert.False(result.Success);
        Assert.Contains(_fakeEvaluator.ResultToReturn.ErrorMessage, result.ErrorMessage);
    }
}

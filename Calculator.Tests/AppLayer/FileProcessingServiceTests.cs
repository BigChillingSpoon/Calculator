using Calculator.AppLayer.Interfaces;
using Calculator.AppLayer.Models;
using Calculator.AppLayer.Models.Enums;
using Calculator.AppLayer.Services;
using Calculator.Core.Interfaces;
using Calculator.IO.Services.Interfaces;
using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Unit tests for FileProcessingService
/// </summary>
namespace Calculator.Tests.AppLayer;
public class FileProcessingServiceTests
{
    private class FakeValidator : IUserFileInputValidator
    {
        public ProcessResult ResultToReturn { get; set; } = new() { Success = true };

        public ProcessResult ProcessUserFileInputs(string inputPath, string outputDirPath, string outputFileName)
            => ResultToReturn;
    }

    private class FakeEvaluator : IExpressionEvaluationService
    {
        public Dictionary<string, Calculator.Core.Models.ParsingResult> Map { get; } = new();

        public Calculator.Core.Models.ParsingResult Evaluate(string expr)
        {
            if (Map.ContainsKey(expr))
                return Map[expr];

            return new Calculator.Core.Models.ParsingResult
            {
                Success = false,
                ErrorMessage = "Unknown expression"
            };
        }
    }

    private class FakeFileService : IFileService
    {
        public List<string> LinesToReturn { get; set; } = new();
        public bool ThrowFileNotFound { get; set; }

        public Task AppendLineAsync(string path, string line)
        {
            return Task.CompletedTask;
        }

        public Task CreateEmptyFileAsync(string path)
        {
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<string> GetFileLinesAsync(string path)
        {
            if (ThrowFileNotFound)
                throw new FileNotFoundException();

            foreach (var line in LinesToReturn)
                yield return line;
        }
    }

    private readonly FakeFileService _file;
    private readonly FakeEvaluator _eval;
    private readonly FakeValidator _validator;
    private readonly FileProcessingService _service;

    public FileProcessingServiceTests()
    {
        _file = new FakeFileService();
        _eval = new FakeEvaluator();
        _validator = new FakeValidator();

        _service = new FileProcessingService(_file, _eval, _validator);
    }

    [Fact]
    public async Task ProcessEvaluationFromFileAsync_ReturnsValidationFailure()
    {
        // Arrange
        _validator.ResultToReturn = new ProcessResult
        {
            Success = false,
            ErrorMessage = "Validation error"
        };

        // Act
        var result = await _service.ProcessEvaluationFromFileAsync("in", "out", "file");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Validation error", result.ErrorMessage);
    }

    [Fact]
    public async Task ProcessEvaluationFromFileAsync_ProcessesLines()
    {
        // Arrange
        _validator.ResultToReturn = new ProcessResult { Success = true };

        _file.LinesToReturn = new() { "1+1", "3+2" };

        _eval.Map["1+1"] = new() { Success = true, Value = "2" };
        _eval.Map["3+2"] = new() { Success = true, Value = "5" };

        // Act
        var result = await _service.ProcessEvaluationFromFileAsync("input.txt", "out", "result");

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task ProcessEvaluationFromFileAsync_ConvertsFileNotFoundToFriendlyError()
    {
        // Arrange
        _validator.ResultToReturn = new ProcessResult { Success = true };
        _file.ThrowFileNotFound = true;

        // Act
        var result = await _service.ProcessEvaluationFromFileAsync("in", "out", "file");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Input file not found", result.ErrorMessage);
    }
}

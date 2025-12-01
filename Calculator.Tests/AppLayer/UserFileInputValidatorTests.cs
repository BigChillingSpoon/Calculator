using Calculator.AppLayer.Validators;
using Xunit;
using System.IO;

/// <summary>
/// Unit tests for UserFileInputValidator
/// </summary>
namespace Calculator.Tests.AppLayer;
public class UserFileInputValidatorTests
{
    private readonly UserFileInputValidator _validator;

    public UserFileInputValidatorTests()
    {
        _validator = new UserFileInputValidator();
    }

    [Fact]
    public void ProcessUserFileInputs_Fails_WhenInputPathEmpty()
    {
        var result = _validator.ProcessUserFileInputs("", "C:\\output");

        Assert.False(result.Success);
        Assert.Contains("empty", result.ErrorMessage);
    }

    [Fact]
    public void ProcessUserFileInputs_Fails_WhenInputNotTxt()
    {
        var result = _validator.ProcessUserFileInputs("input.pdf", "C:\\output");

        Assert.False(result.Success);
        Assert.Contains(".txt file", result.ErrorMessage);
    }

    [Fact]
    public void ProcessUserFileInputs_Fails_WhenOutputEmpty()
    {
        var result = _validator.ProcessUserFileInputs("input.txt", "");

        Assert.False(result.Success);
        Assert.Contains("path is empty", result.ErrorMessage);
    }

    [Fact]
    public void ProcessUserFileInputs_Fails_WhenInputPathInvalid()
    {
        var result = _validator.ProcessUserFileInputs("**|invalid.txt", "C:\\output");

        Assert.False(result.Success);
        Assert.Contains("valid path", result.ErrorMessage);
        Assert.Contains("input", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);//shall always contain information about input
    }

    [Fact]
    public void ProcessUserFileInputs_Fails_WhenInputFileNotExists()
    {
        var result = _validator.ProcessUserFileInputs("nonexistent.txt", Path.GetTempPath());

        Assert.False(result.Success);
        Assert.Equal("Input file does not exist", result.ErrorMessage);
    }
}
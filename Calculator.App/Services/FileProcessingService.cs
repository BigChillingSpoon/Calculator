using Calculator.AppLayer.Models;
using Calculator.AppLayer.Models.Enums;
using Calculator.AppLayer.Services.Interfaces;
using Calculator.Core.Interfaces;
using Calculator.IO.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.AppLayer.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IFileService _fileService;
        private readonly IExpressionEvaluator _expressionEvaluator;
        public FileProcessingService(IFileService fileService, IExpressionEvaluator expressionEvaluator)
        {
            _fileService = fileService;
            _expressionEvaluator = expressionEvaluator;
        }

        public async Task<ProcessResult> ProcessEvaluationFromFileAsync(string inputFilePath, string outputDirectoryPath)
        {
            var validationResult = ProcessUserInputs(inputFilePath, outputDirectoryPath);
            if (!validationResult.Success)
                return validationResult;

            var outputLines = new List<string>();
            try
            {
                await foreach (var line in _fileService.GetFileLinesAsync(inputFilePath))
                {
                    if(string.IsNullOrEmpty(line))
                        continue;//ignoration of empty lines
                    
                    var evaluationResult = _expressionEvaluator.Evaluate(line);
                    var evaluationOutput = evaluationResult.Success ? evaluationResult.Value.ToString() : evaluationResult.ErrorMessage;
                    outputLines.Add(evaluationOutput);
                }

                await _fileService.SaveLinesToDirectoryAsync(outputDirectoryPath, outputLines);

                return new ProcessResult
                {
                    Success = true,
                    ErrorType = ErrorType.None
                };
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private bool IsTxtFile(string inputFilePath)
        {
            return inputFilePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
        }

        private ProcessResult ProcessUserInputs(string inputPath, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return new ProcessResult
                {
                    Success = false,
                    ErrorMessage = "Input file path is empty",
                    ErrorType = ErrorType.Warning
                };
            }

            if(!IsTxtFile(inputPath))
            {
                return new ProcessResult
                {
                    Success = false,
                    ErrorMessage = "Input file must be a .txt file",
                    ErrorType = ErrorType.Warning
                };
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                return new ProcessResult
                {
                    Success = false,
                    ErrorMessage = "Output directory path is empty",
                    ErrorType = ErrorType.Warning
                };
            }

            return new ProcessResult
            {
                Success = true,
                ErrorType = ErrorType.None
            };
        }
        private ProcessResult HandleException(Exception ex)
        {
            return ex switch
            {
                FileNotFoundException => new ProcessResult
                {
                    Success = false,
                    ErrorMessage = "Input file not found",
                    ErrorType = ErrorType.Error
                },

                UnauthorizedAccessException => new ProcessResult
                {
                    Success = false,
                    ErrorMessage = "Access denied to file",
                    ErrorType = ErrorType.Error
                },

                IOException ioEx => new ProcessResult
                {
                    Success = false,
                    ErrorMessage = $"IO error: {ioEx.Message}",
                    ErrorType = ErrorType.Error
                },

                _ => new ProcessResult
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                    ErrorType = ErrorType.Error
                }
            };
        }
    }
}

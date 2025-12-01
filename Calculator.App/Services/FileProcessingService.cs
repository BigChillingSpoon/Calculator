using Calculator.AppLayer.Models;
using Calculator.AppLayer.Models.Enums;
using Calculator.AppLayer.Interfaces;
using Calculator.Core.Interfaces;
using Calculator.IO.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Calculator.AppLayer.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IFileService _fileService;
        private readonly IExpressionEvaluationService _expressionEvaluator;
        private readonly IUserFileInputValidator _inputValidator;
        private const string DefaultFileExtension = ".txt";
        public FileProcessingService(IFileService fileService, IExpressionEvaluationService expressionEvaluator, IUserFileInputValidator inputValidator)
        {
            _fileService = fileService;
            _expressionEvaluator = expressionEvaluator;
            _inputValidator = inputValidator;
        }
        /// <summary>
        /// Processes evaluation of expressions from an input file and saves the results to an output file in the specified directory.
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputDirectoryPath"></param>
        /// <param name="outputFileName"></param>
        /// <returns>ProcessResult success whether all evaluations were done, otherwise returns faliure with user friendly message.</returns>
        public async Task<ProcessResult> ProcessEvaluationFromFileAsync( string inputFilePath, string outputDirectoryPath, string outputFileName)
        {
            // validate input values
            var validation = _inputValidator.ProcessUserFileInputs(inputFilePath, outputDirectoryPath);
            if (!validation.Success)
                return validation;

            // prepare output filename
            outputFileName = PrepareOutputFileName(outputFileName);

            try
            {
                // lazily read lines from input file
                var outputLines = new List<string>();

                await foreach (var line in _fileService.GetFileLinesAsync(inputFilePath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var evaluation = _expressionEvaluator.Evaluate(line);

                    string output = evaluation.Success ? evaluation.Value : evaluation.ErrorMessage;

                    outputLines.Add(output);
                }

                // let IO layer write the full content
                await _fileService.SaveLinesToDirectoryAsync(
                    outputDirectoryPath,
                    outputLines,
                    outputFileName);

                return Success();
            }
            catch (FileNotFoundException)
            {
                return Fail("Input file not found", ErrorType.Error);
            }
            catch (UnauthorizedAccessException)
            {
                return Fail("Access denied to the file system", ErrorType.Error);
            }
            catch (IOException ioEx)
            {
                return Fail($"IO error: {ioEx.Message}", ErrorType.Error);
            }
        }

        private string PrepareOutputFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return $"Output_{DateTime.Now:yyyyMMdd_HHmmss}{DefaultFileExtension}";

            if (!name.EndsWith(DefaultFileExtension, StringComparison.OrdinalIgnoreCase))
                name += DefaultFileExtension;

            return name;
        }
        
        private ProcessResult Fail(string message, ErrorType type)
            => new ProcessResult { Success = false, ErrorMessage = message, ErrorType = type };

        private ProcessResult Success()
            => new ProcessResult { Success = true, ErrorType = ErrorType.None };
    }
}

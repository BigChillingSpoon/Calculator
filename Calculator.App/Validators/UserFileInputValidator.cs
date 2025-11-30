using Calculator.AppLayer.Interfaces;
using Calculator.AppLayer.Models.Enums;
using Calculator.AppLayer.Models;
using Calculator.Core.Interfaces;
using Calculator.IO.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.AppLayer.Validators
{
    public class UserFileInputValidator : IUserFileInputValidator
    {
        public ProcessResult ProcessUserFileInputs(string inputPath, string outputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
                return Fail("Input file path is empty", ErrorType.Warning);

            if (!inputPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                return Fail("Input file must be a .txt file", ErrorType.Warning);

            if (string.IsNullOrWhiteSpace(outputPath))
                return Fail("Output directory path is empty", ErrorType.Warning);

            // Validate path format
            if (!IsValidPath(inputPath))
                return Fail("Input file path is not a valid path format", ErrorType.Warning);

            if (!IsValidPath(outputPath))
                return Fail("Output directory path is not a valid path format", ErrorType.Warning);

            // Check file exists
            if (!File.Exists(inputPath))
                return Fail("Input file does not exist", ErrorType.Error);

            // Check directory exists
            if (!Directory.Exists(outputPath))
                return Fail("Output directory does not exist", ErrorType.Error);

            // Check write permission
            if (!CanWriteToDirectory(outputPath))
                return Fail("Cannot write to output directory", ErrorType.Error);

            return Success();
        }

        private bool IsValidPath(string path)
        {
            try
            {
                Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CanWriteToDirectory(string directory)
        {
            try
            {
                var testFile = Path.Combine(directory, "__write_test.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private ProcessResult Fail(string message, ErrorType type)
             => new ProcessResult { Success = false, ErrorMessage = message, ErrorType = type };

        private ProcessResult Success()
            => new ProcessResult { Success = true, ErrorType = ErrorType.None };
    }
}


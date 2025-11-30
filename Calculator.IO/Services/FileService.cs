using Calculator.IO.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.IO.Services
{
    public class FileService : IFileService
    {
        public async IAsyncEnumerable<string> GetFileLinesAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Input file not found.", path);

            using var reader = new StreamReader(path);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                yield return line;
            }
        }

        public async Task SaveLinesToDirectoryAsync(string directoryPath, IEnumerable<string> lines, string ouputFileName)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Output directory not found: {directoryPath}");
            if (string.IsNullOrWhiteSpace(ouputFileName))
                throw new ArgumentException("Output file name cannot be null or whitespace.", nameof(ouputFileName));
            
            var outputFilePath = Path.Combine(directoryPath, ouputFileName);

            await using var writer = new StreamWriter(
                new FileStream(
                    outputFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 8192,
                    useAsync: true));

            foreach (var line in lines)
            {
                await writer.WriteLineAsync(line);
            }
        }
    }
}

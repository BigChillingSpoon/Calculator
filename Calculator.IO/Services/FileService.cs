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
        /// <summary>
        /// If file does not exist creates new file, if file already exists clears the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task CreateEmptyFileAsync(string path)
        {
            await using var writer = new StreamWriter(
                new FileStream(
                    path,
                    FileMode.Create,    // ALWAYS clears
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 8192,
                    useAsync: true));

            await writer.FlushAsync();
        }

        /// <summary>
        /// Appends line to given file  
        /// </summary>
        /// <param name="path"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public async Task AppendLineAsync(string path, string line)
        {
            await using var writer = new StreamWriter(
                new FileStream(
                    path,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.Read,
                    bufferSize: 8192,
                    useAsync: true));

            await writer.WriteLineAsync(line);
        }
    }
}

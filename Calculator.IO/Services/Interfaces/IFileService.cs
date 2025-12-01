using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.IO.Services.Interfaces
{
    public interface IFileService
    {
        public IAsyncEnumerable<string> GetFileLinesAsync(string filePath);
        Task CreateEmptyFileAsync(string path);
        Task AppendLineAsync(string path, string line);
    }
}

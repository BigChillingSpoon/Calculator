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
        public Task SaveLinesToDirectoryAsync(string directoryPath, IEnumerable<string> lines);
    }
}

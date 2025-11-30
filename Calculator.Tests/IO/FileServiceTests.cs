using Calculator.IO.Services;
using Xunit;

public class FileServiceTests
{
    private readonly FileService _fileService;
    public FileServiceTests()
    {
        _fileService = new FileService();
    }

    #region Get
    [Fact]
    public async Task GetFileLinesAsync_ReturnsAllLines()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        var lines = new[] { "A", "B", "C" };
        await File.WriteAllLinesAsync(tempFile, lines);

        // Act
        var result = new List<string>();
        await foreach (var line in _fileService.GetFileLinesAsync(tempFile))
        {
            result.Add(line);
        }

        // Assert
        Assert.Equal(lines, result);
    }

    [Fact]
    public async Task GetFileLinesAsync_Throws_When_FileNotFound()
    {
        // Arrange
        var invalidPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");

        // Act + Assert
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await foreach (var _ in _fileService.GetFileLinesAsync(invalidPath))
            {
                // enumerating triggers the exception
            }
        });
    }
    #endregion Get
    #region Saving
    [Fact]
    public async Task SaveLinesToDirectoryAsync_WritesLinesToOutputFile()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory().FullName;
        var fileName = "output.txt";
        var lines = new[] { "Line1", "Line2" };

        var outputPath = Path.Combine(tempDir, fileName);

        // Act
        await _fileService.SaveLinesToDirectoryAsync(tempDir, lines, fileName);

        // Assert
        Assert.True(File.Exists(outputPath));
        var fileContent = await File.ReadAllLinesAsync(outputPath);
        Assert.Equal(lines, fileContent);
    }

    [Fact]
    public async Task SaveLinesToDirectoryAsync_Throws_When_DirectoryNotFound()
    {
        // Arrange
        var invalidDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act + Assert
        await Assert.ThrowsAsync<DirectoryNotFoundException>(async () =>
        {
            await _fileService.SaveLinesToDirectoryAsync(invalidDir, new[] { "A" }, "file.txt");
        });
    }

    [Fact]
    public async Task SaveLinesToDirectoryAsync_Throws_When_FilenameInvalid()
    {
        // Arrange
        var tempDir = Directory.CreateTempSubdirectory().FullName;

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _fileService.SaveLinesToDirectoryAsync(tempDir, new[] { "X" }, "");
        });
    }
    #endregion Saving
}

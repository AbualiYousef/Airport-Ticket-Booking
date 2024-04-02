using DataAccess.CsvHelperService;
using FluentAssertions;

namespace ATB.Tests.CsvFileService;

public class CsvFileServiceTests
{
    #region ReadFromCsvAsync

    [Fact]
    public async Task ReadFromCsvAsync_NonExistentFile_ShouldThrowException()
    {
        var csvFileService = new CsvFileService<TestData>();
        var nonExistentFilePath = Path.Combine(Path.GetTempPath(), "nonexistent_file.csv");
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await csvFileService.ReadFromCsvAsync(nonExistentFilePath));
    }

    [Fact]
    public async Task ReadFromCsvAsync_EmptyFile_ShouldReturnEmptyList()
    {
        var tempFilePath = Path.GetTempFileName();
        var csvFileService = new CsvFileService<TestData>();

        var result = await csvFileService.ReadFromCsvAsync(tempFilePath);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadFromCsvAsync_ValidData_ShouldReturnCorrectData()
    {
        var tempFilePath = Path.GetTempFileName();
        await File.WriteAllLinesAsync(tempFilePath,
            new[] { "Id,Name", "1,Test 1", "2,Test 2" });
        var csvFileService = new CsvFileService<TestData>();

        var result = await csvFileService.ReadFromCsvAsync(tempFilePath);

        var expected = new List<TestData>
        {
            new TestData { Id = 1, Name = "Test 1" },
            new TestData { Id = 2, Name = "Test 2" }
        };
        result.Should().BeEquivalentTo(expected);
        File.Delete(tempFilePath);
    }

    #endregion

    #region WriteToCsvAsync

    [Theory]
    [InlineData(1, "Yousef")]
    [InlineData(2, "Test")]
    public async Task WriteToCsvAsync_ValidData_ShouldWriteCorrectData(int a, string b)
    {
        var tempFilePath = Path.GetTempFileName();
        var csvFileService = new CsvFileService<TestData>();
        var data = new List<TestData>
        {
            new TestData { Id = a, Name = b },
            new TestData { Id = a, Name = b }
        };

        await csvFileService.WriteToCsvAsync(tempFilePath, data);

        var result = await csvFileService.ReadFromCsvAsync(tempFilePath);
        result.Should().BeEquivalentTo(data);
        File.Delete(tempFilePath);
    }

    [Fact]
    public async Task WriteToCsvAsync_EmptyData_ShouldWriteEmptyFile()
    {
        var tempFilePath = Path.GetTempFileName();
        var csvFileService = new CsvFileService<TestData>();
        var data = new List<TestData>();

        await csvFileService.WriteToCsvAsync(tempFilePath, data);

        var result = await csvFileService.ReadFromCsvAsync(tempFilePath);
        result.Should().BeEmpty();
        File.Delete(tempFilePath);
    }

    [Fact]
    public async Task WriteToCsvAsync_NonExistentDirectory_ShouldThrowException()
    {
        var csvFileService = new CsvFileService<TestData>();
        var nonExistentDirectoryPath = Path.Combine(Path.GetTempPath(), "nonexistent_directory");
        var nonExistentFilePath = Path.Combine(nonExistentDirectoryPath, "nonexistent_file.csv");
        var data = new List<TestData>();

        await Assert.ThrowsAsync<DirectoryNotFoundException>(async () =>
            await csvFileService.WriteToCsvAsync(nonExistentFilePath, data));
    }

    #endregion
}
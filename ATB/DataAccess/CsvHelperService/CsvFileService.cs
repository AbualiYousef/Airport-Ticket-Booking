using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataAccess.CsvHelperService;

public class CsvFileService<T> : ICsvFileService<T>
{
    private readonly CsvConfiguration _config = new(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
    };

    public Task<List<T>> ReadFromCsvAsync(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, _config);
        var records =  csv.GetRecords<T>().ToList();
        return Task.FromResult(records);;
    }
    
    public async Task WriteToCsvAsync(string filePath, IEnumerable<T> records)
    {
        await using var writer = new StreamWriter(filePath);
        await using var csv = new CsvWriter(writer, _config);
        await csv.WriteRecordsAsync(records);
    }
}
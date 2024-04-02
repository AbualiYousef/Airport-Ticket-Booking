namespace DataAccess.CsvHelperService;

public interface ICsvFileService<T>
{
    Task<List<T>> ReadFromCsvAsync(string filePath);
    Task WriteToCsvAsync(string filePath, IEnumerable<T> records);
}
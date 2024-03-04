namespace DataAccess.Csv;

public interface ICsvWritable
{
    string GetCsvRecord();

    static abstract string GetHeader();
}
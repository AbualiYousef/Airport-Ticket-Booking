using DataAccess.Csv;

namespace DataAccess.Models;

public class Passenger: ICsvWritable
{
    public int Id { get; init; }
  
    public string Name { get; init; }
    
    public string GetCsvRecord() =>
        $"{Id},{Name}";

    public static string GetHeader() => ModelsCsvHeaders.PassengerCsvHeader;
    
}//End of Passenger class
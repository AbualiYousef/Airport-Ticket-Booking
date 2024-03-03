using DataAccess.Csv;

namespace DataAccess.Models;

public class Flight : ICsvWritable
{
    public int Id { get; set; }

    public string DepartureCountry { get; set; }

    public string DestinationCountry { get; set; }

    public DateTime DepartureDate { get; set; }

    public string DepartureAirport { get; set; }

    public string ArrivalAirport { get; set; }

    public List<FlightClassDetails> ClassDetails { get; set; }


    public string GetCsvRecord()
    {
        var flightClasses = string.Join(';', ClassDetails.Select(details => (int)details.Class));

        var flightCapacities = string.Join(';', ClassDetails.Select(details => details.Capacity));

        var flightPrices = string.Join(';', ClassDetails.Select(details => details.Price));

        return
            $"{Id},{DepartureCountry},{DestinationCountry},{DepartureDate},{DepartureAirport},{ArrivalAirport}" +
            $",{flightClasses},{flightCapacities},{flightPrices}";
    }

    public static string GetHeader() => ModelsCsvHeaders.FlightCsvHeader;
} //End of Flight class
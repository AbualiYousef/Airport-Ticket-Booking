namespace DataAccess.Dtos;

public class FlightDto
{
    
    public int Id { get; set; }
    
    public string DepartureCountry { get; set; }

    public string DestinationCountry { get; set; }

    public DateTime DepartureDate { get; set; }

    public string DepartureAirport { get; set; }

    public string ArrivalAirport { get; set; }

    public List<FlightClassDetailsDto> ClassDetails { get; set; }
} //End of FlightDto class
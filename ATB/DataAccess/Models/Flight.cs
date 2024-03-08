namespace DataAccess.Models;

public class Flight 
{
    public Guid Id { get; set; }

    public string DepartureCountry { get; set; }

    public string DestinationCountry { get; set; }

    public DateTime DepartureDate { get; set; }

    public string DepartureAirport { get; set; }

    public string ArrivalAirport { get; set; }

    public List<FlightClassDetails> ClassDetails { get; set; }
    
} //End of Flight class

using DataAccess.Enums;
using DataAccess.Models;

namespace DataAccess.SearchCriteria;

public class FlightSearchCriteria
{
    public decimal? Price { get; set; }
 
    public string? DepartureCountry { get; set; }
 
    public string? DestinationCountry { get; set; }
 
    public DateTime? DepartureDate { get; set; }
 
    public string? DepartureAirport { get; set; }
 
    public string? ArrivalAirport { get; set; }
 
    public FlightClass? Class { get; set; }

    public bool Matches(Flight flight)
    {
        var departureCountryMatches = DepartureCountry == null || flight.DepartureCountry.Equals(DepartureCountry);
        var destinationCountryMatches = DestinationCountry == null || flight.DestinationCountry.Equals(DestinationCountry);
        var departureDateMatches = (DepartureDate ?? flight.DepartureDate) == flight.DepartureDate;
        var departureAirportMatches = DepartureAirport == null || flight.DepartureAirport.Equals(DepartureAirport);
        var arrivalAirportMatches = ArrivalAirport == null || flight.ArrivalAirport.Equals(ArrivalAirport);
        var priceMatches = Price == null || flight.ClassDetails.Any(details => details.Price == Price);
        var classMatches = Class == null || flight.ClassDetails.Any(details => details.Class == Class);

        return departureCountryMatches &&
               destinationCountryMatches &&
               departureDateMatches &&
               departureAirportMatches &&
               arrivalAirportMatches &&
               priceMatches &&
               classMatches;
    }
}
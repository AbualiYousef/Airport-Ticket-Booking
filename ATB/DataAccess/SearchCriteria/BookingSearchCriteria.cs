using DataAccess.Enums;

namespace DataAccess.SearchCriteria;

public class BookingSearchCriteria
{
    public int? PassengerId { get; set; }

    public int? FlightId { get; set; }

    public decimal? Price { get; set; }

    public string? DepartureCountry { get; set; }

    public string? DestinationCountry { get; set; }

    public DateTime? DepartureDate { get; set; }

    public string? DepartureAirport { get; set; }

    public string? ArrivalAirport { get; set; }

    public FlightClass? Class { get; set; }

    public bool Matches(Booking booking)
    {
        var passengerIdMatches = (PassengerId ?? booking.Passenger.Id) == booking.Passenger.Id;
        var flightIdMatches = (FlightId ?? booking.Flight.Id) == booking.Flight.Id;
        var departureCountryMatches =
            DepartureCountry == null || booking.Flight.DepartureCountry.Equals(DepartureCountry);
        var destinationCountryMatches =
            DestinationCountry == null || booking.Flight.DestinationCountry.Equals(DestinationCountry);
        var departureDateMatches = (DepartureDate ?? booking.Flight.DepartureDate) == booking.Flight.DepartureDate;
        var departureAirportMatches =
            DepartureAirport == null || booking.Flight.DepartureAirport.Equals(DepartureAirport);
        var arrivalAirportMatches = ArrivalAirport == null || booking.Flight.ArrivalAirport.Equals(ArrivalAirport);
        var priceMatches = Price == null ||
                           booking.Flight.ClassDetails.First(details => details.Class == booking.BookingClass).Price ==
                           Price;
        var classMatches = Class == null || booking.BookingClass == Class;

        return passengerIdMatches &&
               flightIdMatches &&
               departureCountryMatches &&
               destinationCountryMatches &&
               departureDateMatches &&
               departureAirportMatches &&
               arrivalAirportMatches &&
               priceMatches &&
               classMatches;
    }
} //End of BookingSearchCriteria class
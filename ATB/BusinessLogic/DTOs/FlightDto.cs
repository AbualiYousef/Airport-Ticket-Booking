using DataAccess.Models;

namespace BusinessLogic.DTOs;

public class FlightDto(Flight flight)
{
    public Guid Id { get; init; } = flight.Id;
    public string DepartureCountry { get; init; } = flight.DepartureCountry;

    public string DestinationCountry { get; init; } = flight.DestinationCountry;

    public DateTime DepartureDate { get; init; } = flight.DepartureDate;

    public string DepartureAirport { get; init; } = flight.DepartureAirport;

    public string ArrivalAirport { get; init; } = flight.ArrivalAirport;

    public IList<FlightClassDetailsDto> ClassDetails { get; init; }
        = flight
            .ClassDetails
            .Select(details => new FlightClassDetailsDto(details))
            .ToList();


    public override string ToString()
    {
        return
            $"Id: {Id}, DepartureCountry: {DepartureCountry}, " +
            $"DestinationCountry: {DestinationCountry}, DepartureDate: {DepartureDate}, " +
            $"DepartureAirport: {DepartureAirport}, ArrivalAirport: {ArrivalAirport}, " +
            $"ClassDetails: {string.Join(", ", ClassDetails)}";
    }
}
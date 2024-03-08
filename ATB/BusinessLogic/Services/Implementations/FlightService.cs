using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;

namespace BusinessLogic.Services.Implementations;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;

    private readonly IBookingRepository _bookingRepository;

    // private readonly IImportFromCsvService<FlightCsvImportDto, Flight> _importFromCsvService;

    public FlightService(IFlightRepository flightRepository, IBookingRepository bookingRepository)
    {
        _flightRepository = flightRepository;
        _bookingRepository = bookingRepository;
    }


    public async Task<FlightDto> GetByIdAsync(int id)
    {
        var flight = await _flightRepository.GetByIdAsync(id);
        if (flight == null)
        {
            throw new ArgumentException($"Flight with id {id} not found");
        }

        return new FlightDto(flight);
    }

    public async Task<IEnumerable<FlightDto>> GetAllAsync()
    {
        var flights = await _flightRepository.GetAllAsync();
        return flights.Select(f => new FlightDto(f));
    }

    public async Task<IEnumerable<FlightDto>> GetMatchingCriteriaAsync(FlightSearchCriteria criteria)
    {
        var flightsMatchingCriteria =
            await _flightRepository.GetMatchingCriteriaAsync(criteria);

        if (criteria.Class is null)
        {
            return flightsMatchingCriteria
                .Where(f => f
                    .ClassDetails.Exists(d => IsClassAvailableToBook(f, d.Class).Result))
                .Select(flight => new FlightDto(flight));
        }

        return flightsMatchingCriteria
            .Where(f => IsClassAvailableToBook(f, criteria.Class.Value).Result)
            .Select(flight => new FlightDto(flight));
    }

    private async Task<bool> IsClassAvailableToBook(Flight bookingFlight, FlightClass newClass)
    {
        var capacity = bookingFlight
            .ClassDetails
            .First(details => details.Class == newClass)
            .Capacity;

        var bookedSeats = (await _bookingRepository
                .GetBookingsForFlightWithClassAsync(bookingFlight.Id, newClass))
            .Count();

        return capacity - bookedSeats > 0;
    }
}
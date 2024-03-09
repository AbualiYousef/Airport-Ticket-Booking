using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using DataAccess.CsvHelperService;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;
using DataAccess.Validation;

namespace BusinessLogic.Services.Implementations;

public class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;

    private readonly IBookingRepository _bookingRepository;

    private readonly ICsvFileService<Flight> _csvFileService;

    public FlightService(IFlightRepository flightRepository, IBookingRepository bookingRepository,
        ICsvFileService<Flight> csvFileService)
    {
        _flightRepository = flightRepository;
        _bookingRepository = bookingRepository;
        _csvFileService = csvFileService;
    }


    public async Task<FlightDto> GetByIdAsync(Guid id)
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

    public async Task<List<string>> ImportFlightsFromCsvAsync(string filePath)
    {
        var flights = await _csvFileService.ReadFromCsvAsync(filePath);
        var flightValidator = new ModelValidator<Flight>();
        var validationErrors = new List<string>();
        var validFlights = new List<Flight>();

        foreach (var flight in flights)
        {
            var errors = flightValidator.Validate(flight);
            if (errors.Any())
            {
                validationErrors.AddRange(errors);
            }
            else
            {
                validFlights.Add(flight);
            }
        }

        if (validFlights.Any())
        {
            await _flightRepository.AddAsync(validFlights);
        }

        return validationErrors;
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
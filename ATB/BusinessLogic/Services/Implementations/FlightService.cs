using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using DataAccess.CsvHelperService;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;
using DataAccess.Validation;

namespace BusinessLogic.Services.Implementations;

public class FlightService(
    IFlightRepository flightRepository,
    IBookingRepository bookingRepository,
    ICsvFileService<Flight> csvFileService)
    : IFlightService
{
    public async Task<FlightDto> GetByIdAsync(Guid id)
    {
        var flight = await flightRepository.GetByIdAsync(id);
        if (flight == null)
        {
            throw new ArgumentException($"Flight with id {id} not found");
        }

        return new FlightDto(flight);
    }

    public async Task<IEnumerable<FlightDto>> GetAllAsync()
    {
        var flights = await flightRepository.GetAllAsync();
        return flights.Select(f => new FlightDto(f));
    }

    public async Task<IEnumerable<FlightDto>> GetAvailableFlightsMatchingCriteria(FlightSearchCriteria criteria)
    {
        var flightsMatchingCriteria =
            await flightRepository.GetMatchingCriteriaAsync(criteria);

        if (criteria.Class is null)
        {
            return flightsMatchingCriteria
                .Where(f => f
                    .ClassDetails.Exists(d => IsClassAvailableToBook(f, d.Class)))
                .Select(flight => new FlightDto(flight));
        }

        return flightsMatchingCriteria
            .Where(f => IsClassAvailableToBook(f, criteria.Class.Value))
            .Select(flight => new FlightDto(flight));
    }

    public async Task<List<string>> ImportFlightsFromCsvAsync(string filePath)
    {
        var flights = await csvFileService.ReadFromCsvAsync(filePath);
        var flightValidator = new ModelValidator<Flight>();
        var validationErrors = new List<string>();
        var validFlights = new List<Flight>();

        foreach (var flight in flights)
        {
            var errors = flightValidator.Validate(flight);
            if (errors.Count != 0)
            {
                validationErrors.AddRange(errors);
            }
            else
            {
                validFlights.Add(flight);
            }
        }

        if (validFlights.Count != 0)
        {
            await flightRepository.AddAsync(validFlights);
        }

        return validationErrors;
    }

    private bool IsClassAvailableToBook(Flight bookingFlight, FlightClass newClass)
    {
        var classDetail = bookingFlight
            .ClassDetails
            .FirstOrDefault(details => details.Class == newClass);

        if (classDetail == null)
        {
            return false;
        }

        var capacity = classDetail.Capacity;

        var bookings = bookingRepository.GetBookingsForFlightWithClassAsync(bookingFlight.Id, newClass).Result;

        var bookedSeats = bookings.Count();

        return capacity - bookedSeats > 0;
    }
}
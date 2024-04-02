using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.SearchCriteria;

namespace DataAccess.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();

    Task<Booking?> GetByIdAsync(Guid id);

    Task AddAsync(Booking booking);

    Task UpdateAsync(Booking booking);

    Task DeleteAsync(Booking booking);

    Task<List<Booking>> GetPassengerBookingsAsync(Guid passengerId);

    Task<List<Booking>> GetBookingsForFlightWithClassAsync(Guid flightId, FlightClass flightClass);

    Task<List<Booking>> GetMatchingCriteriaAsync(BookingSearchCriteria? criteria);
} 
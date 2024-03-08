using DataAccess.Enums;
using DataAccess.SearchCriteria;

namespace DataAccess.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();

    Task<Booking?> GetByIdAsync(Guid id);

    Task AddAsync(Booking booking);

    Task UpdateAsync(Booking booking);

    Task DeleteAsync(Booking booking);

    Task<IEnumerable<Booking>> GetPassengerBookingsAsync(Guid passengerId);

    Task<IEnumerable<Booking>> GetBookingsForFlightWithClassAsync(Guid flightId, FlightClass flightClass);

    Task<IEnumerable<Booking>> GetMatchingCriteriaAsync(BookingSearchCriteria criteria);
} //End of IBookingRepository interface
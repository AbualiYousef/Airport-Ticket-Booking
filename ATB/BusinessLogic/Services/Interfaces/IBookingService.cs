using BusinessLogic.DTOs;
using DataAccess.Enums;
using DataAccess.SearchCriteria;

namespace BusinessLogic.Services.Interfaces;

public interface IBookingService
{
    Task<BookingDto?> GetByIdAsync(Guid id);
    Task BookFlight(Guid flightId, Guid passengerId, FlightClass flightClass);
    Task CancelBooking(Guid bookingId);
    Task ModifyBooking(Guid bookingId, FlightClass newClass);
    Task<IEnumerable<BookingDto>> GetPassengerBookingsAsync(Guid passengerId);
    Task<IEnumerable<BookingDto>> GetMatchingCriteriaAsync(BookingSearchCriteria criteria);
}
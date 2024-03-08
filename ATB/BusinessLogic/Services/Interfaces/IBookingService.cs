using BusinessLogic.DTOs;
using DataAccess.Enums;
using DataAccess.SearchCriteria;

namespace BusinessLogic.Services.Interfaces;

public interface IBookingService
{
    Task<BookingDto?> GetByIdAsync(int id);
    Task BookFlight(int flightId, int passengerId, FlightClass flightClass);
    Task CancelBooking(int bookingId);
    Task ModifyBooking(int bookingId, FlightClass newClass);
    Task<IEnumerable<BookingDto>> GetPassengerBookingsAsync(int passengerId);
    Task<IEnumerable<BookingDto>> GetMatchingCriteriaAsync(BookingSearchCriteria criteria);
}
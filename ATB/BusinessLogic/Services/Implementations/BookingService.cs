using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;

namespace BusinessLogic.Services.Implementations;

public class BookingService(
    IBookingRepository bookingRepository,
    IFlightRepository flightRepository,
    IPassengerRepository passengerRepository)
    : IBookingService
{
    public async Task<BookingDto?> GetByIdAsync(Guid id)
    {
        var booking = await bookingRepository.GetByIdAsync(id);
        return booking is null ? null : new BookingDto(booking);
    }

    public async Task BookFlight(Guid flightId, Guid passengerId, FlightClass flightClass)
    {
        var flight = await flightRepository.GetByIdAsync(flightId);
        if (flight == null)
        {
            throw new ArgumentException($"Flight with id {flightId} not found");
        }

        if (!(await IsClassAvailableToBook(flight, flightClass)))
        {
            throw new ArgumentException($"Class {flightClass} not available for flight with id {flightId}");
        }

        var passenger = await passengerRepository.GetByIdAsync(passengerId);
        if (passenger == null)
        {
            throw new ArgumentException($"Passenger with id {passengerId} not found");
        }

        var newBooking = new Booking
        {
            Id = Guid.NewGuid(),
            Passenger = passenger,
            Flight = flight,
            BookingClass = flightClass,
            BookingDate = DateTime.Now
        };

        await bookingRepository.AddAsync(newBooking);
    }

    public async Task CancelBooking(Guid bookingId)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            throw new ArgumentException($"Booking with id {bookingId} not found");
        }

        await bookingRepository.DeleteAsync(booking);
    }

    public async Task ModifyBooking(Guid bookingId, FlightClass newClass)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            throw new ArgumentException($"Booking with id {bookingId} not found");
        }

        if (booking.BookingClass == newClass)
        {
            throw new ArgumentException($"Booking with id {bookingId} already has class {newClass}");
        }

        if (booking.Flight.ClassDetails.All(d => d.Class != newClass))
        {
            throw new ArgumentException($"Class {newClass} not available for flight with id {booking.Flight.Id}");
        }

        booking.BookingClass = newClass;
        await bookingRepository.UpdateAsync(booking);
    }

    public async Task<List<BookingDto>> GetPassengerBookingsAsync(Guid passengerId)
    {
        var passenger = await passengerRepository.GetByIdAsync(passengerId);
        if (passenger == null)
        {
            throw new ArgumentException($"Passenger with id {passengerId} not found");
        }

        var bookings = await bookingRepository.GetPassengerBookingsAsync(passengerId);
        return bookings.Select(b => new BookingDto(b)).ToList();
    }


    public async Task<List<BookingDto>> GetMatchingCriteriaAsync(BookingSearchCriteria criteria)
    {
        var bookingsMatchingCriteria = await bookingRepository.GetMatchingCriteriaAsync(criteria);
        return bookingsMatchingCriteria.Select(b => new BookingDto(b)).ToList();
    }

    private async Task<bool> IsClassAvailableToBook(Flight bookingFlight, FlightClass newClass)
    {
        var classDetail = bookingFlight
            .ClassDetails
            .FirstOrDefault(details => details.Class == newClass);
    
        if (classDetail == null)
        {
            return false;
        }
    
        var capacity = classDetail.Capacity;
    
        var bookings = await bookingRepository.GetBookingsForFlightWithClassAsync(bookingFlight.Id, newClass);
    
        var bookedSeats = bookings.Count();
    
        return capacity - bookedSeats > 0;
    }
  
}
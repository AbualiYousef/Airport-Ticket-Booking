using DataAccess.Enums;
using DataAccess.Models;

namespace BusinessLogic.DTOs;

public class BookingDto(Booking booking)
{
    public Guid Id { get; init; } = booking.Id;

    public PassengerDto Passenger { get; init; } = new(booking.Passenger);

    public FlightDto Flight { get; init; } = new(booking.Flight);

    public FlightClass BookingClass { get; init; } = booking.BookingClass;

    public DateTime BookingDate { get; init; } = booking.BookingDate;


    public override string ToString()
    {
        return $"Id: {Id}, Passenger: {Passenger}, Flight: {Flight}," +
               $" BookingClass: {BookingClass}, BookingDate: {BookingDate}";
    }
}
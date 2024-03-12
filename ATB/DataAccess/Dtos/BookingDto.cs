namespace DataAccess.Dtos;

public record BookingDto
{
    public int Id { get; init; }

    public FlightDto Flight { get; init; }

    public PassengerDto Passenger { get; init; }

    public int SeatNumber { get; init; }

    public decimal Price { get; init; }
} //End of BookingDto record

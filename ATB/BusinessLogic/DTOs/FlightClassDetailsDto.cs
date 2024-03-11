using DataAccess.Enums;
using DataAccess.Models;

namespace BusinessLogic.DTOs;

public class FlightClassDetailsDto(FlightClassDetails flightClassDetails)
{
    public FlightClass Class { get; init; } = flightClassDetails.Class;

    public decimal Price { get; init; } = flightClassDetails.Price;

    public int Capacity { get; init; } = flightClassDetails.Capacity;
    
    public override string ToString()
    {
        return $"Class: {Class}, Price: {Price}, Capacity: {Capacity}";
    }
}
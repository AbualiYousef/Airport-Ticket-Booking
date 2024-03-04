using DataAccess.Enums;

namespace DataAccess.Dtos;

public class FlightClassDetailsDto
{
    public FlightClass Class { get; set; }
    public double Price { get; set; }
    public int Capacity { get; set; }
}//End of FlightClassDetails class

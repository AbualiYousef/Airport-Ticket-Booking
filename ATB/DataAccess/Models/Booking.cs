using DataAccess.Enums;
using DataAccess.Models;


public class Booking
{
    public Guid Id { get; set; }

    public Passenger Passenger { get; set; }
  
    public Flight Flight { get; set; }
  
    public FlightClass BookingClass { get; set; }
  
    public DateTime BookingDate { get; set; }
    
}//End of Booking class

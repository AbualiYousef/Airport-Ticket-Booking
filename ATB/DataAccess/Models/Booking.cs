using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;
using DataAccess.Models;


public class Booking
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Passenger information is required")]
    public Passenger Passenger { get; set; }

    [Required(ErrorMessage = "Flight information is required")]
    public Flight Flight { get; set; }

    [Required(ErrorMessage = "Booking class information is required")]
    public FlightClass BookingClass { get; set; }

    [Required(ErrorMessage = "Booking date is required")]
    [Range(typeof(DateTime), "2024-03-08", "2025-03-08",
        ErrorMessage = "Booking date must be between today and one year from now")]
    public DateTime BookingDate { get; set; }
} //End of Booking class
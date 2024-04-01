using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using DataAccess.Enums;

namespace DataAccess.Models;

public class Booking
{
    [Name("BookingId")] 
    public Guid Id { get; set; }

    [Name("PassengerId")]
    [Required(ErrorMessage = "Passenger information is required")]
    public Passenger Passenger { get; set; }

    [Name("FlightId")]
    [Required(ErrorMessage = "Flight information is required")]
    public Flight Flight { get; set; }

    [Required(ErrorMessage = "Booking class information is required")]
    [Name("BookingClass")]
    public FlightClass BookingClass { get; set; }

    [Required(ErrorMessage = "Booking date is required")]
    [Range(typeof(DateTime), "2024-03-08", "2025-03-08",
        ErrorMessage = "Booking date must be between today and one year from now")]
    [Name("BookingDate")]
    public DateTime BookingDate { get; set; }
} 
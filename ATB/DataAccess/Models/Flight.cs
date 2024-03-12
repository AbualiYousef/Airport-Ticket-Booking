using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class Flight
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Departure country is required")]
    [StringLength(50, ErrorMessage = "Departure country cannot be longer than 50 characters")]
    public string DepartureCountry { get; set; }

    [Required(ErrorMessage = "Destination country is required")]
    [StringLength(50, ErrorMessage = "Destination country cannot be longer than 50 characters")]
    public string DestinationCountry { get; set; }

    [Required(ErrorMessage = "Departure date is required")]
    [Range(typeof(DateTime), "2024-03-08", "2025-03-08",
        ErrorMessage = "Departure date must be between today and one year from now")]
    public DateTime DepartureDate { get; set; }

    [Required(ErrorMessage = "Departure airport is required")]
    [StringLength(50, ErrorMessage = "Departure airport cannot be longer than 50 characters")]
    public string DepartureAirport { get; set; }

    [Required(ErrorMessage = "Arrival airport is required")]
    [StringLength(50, ErrorMessage = "Arrival airport cannot be longer than 50 characters")]
    public string ArrivalAirport { get; set; }

    [Required(ErrorMessage = "Class details are required")]
    public List<FlightClassDetails> ClassDetails { get; set; }
} //End of Flight class
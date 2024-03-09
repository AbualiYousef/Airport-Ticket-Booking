using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

public class FlightClassDetails
{
    [Required(ErrorMessage = "Flight class is required")]
    public FlightClass Class { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    [Range(0, 1000, ErrorMessage = "Capacity must be between 0 and 1000")]
    public int Capacity { get; set; }
} //End of FlightClassDetails class
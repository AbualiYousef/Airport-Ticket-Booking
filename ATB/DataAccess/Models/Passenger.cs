using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class Passenger
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; init; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; init; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; init; }

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(056|059)\d{7}$", ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; init; }

    [Required(ErrorMessage = "Passport number is required")]
    [RegularExpression(@"^[A-Z0-9]{6,9}$", ErrorMessage = "Invalid passport number format")]
    public string PassportNumber { get; set; }
} //End of Passenger class
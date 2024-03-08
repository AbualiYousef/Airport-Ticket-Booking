namespace DataAccess.Models;

public class Passenger
{
    public Guid Id { get; init; }

    public string Name { get; init; }
    
    public string Email { get; init; }
    
    public string PhoneNumber { get; init; }
    
    public string PassportNumber { get; set; }
} //End of Passenger class
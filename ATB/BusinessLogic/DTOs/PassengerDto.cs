using DataAccess.Models;

namespace BusinessLogic.DTOs;

public class PassengerDto(Passenger passenger)
{
    public Guid Id { get; init; } = passenger.Id;

    public string Name { get; init; } = passenger.Name;

    public string Email { get; init; } = passenger.Email;

    public string PhoneNumber { get; init; } = passenger.PhoneNumber;

    public string PassportNumber { get; set; } = passenger.PassportNumber;

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Email: {Email}, PhoneNumber: {PhoneNumber}, PassportNumber: {PassportNumber}";
    }
}
using BusinessLogic.DTOs;

namespace BusinessLogic.Services.Interfaces;

public interface IPassengerService
{
    Task<PassengerDto?> GetById(int id);
}
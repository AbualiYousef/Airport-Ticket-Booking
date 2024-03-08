using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using DataAccess.Repositories.Interfaces;

namespace BusinessLogic.Services.Implementations;

public class PassengerService(IPassengerRepository passengerRepository) : IPassengerService
{
    public async Task<PassengerDto?> GetById(int id)
    {
        var passenger = await passengerRepository.GetByIdAsync(id);
        return passenger != null ? new PassengerDto(passenger) : null;
    }
}
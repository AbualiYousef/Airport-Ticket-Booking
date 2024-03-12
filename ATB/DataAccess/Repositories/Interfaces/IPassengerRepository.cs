using DataAccess.Models;

namespace DataAccess.Repositories.Interfaces;

public interface IPassengerRepository
{
    Task<Passenger?> GetByIdAsync(Guid id);
}
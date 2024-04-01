using DataAccess.Models;
using DataAccess.SearchCriteria;

namespace DataAccess.Repositories.Interfaces;

public interface IFlightRepository
{
    Task<List<Flight>> GetAllAsync();

    Task<List<Flight>> GetMatchingCriteriaAsync(FlightSearchCriteria criteria);

    Task<Flight?> GetByIdAsync(Guid id);
    
    Task AddAsync(IEnumerable<Flight> flights);
}
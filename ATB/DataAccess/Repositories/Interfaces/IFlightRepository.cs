using DataAccess.Models;
using DataAccess.SearchCriteria;

namespace DataAccess.Repositories.Interfaces;

public interface IFlightRepository
{
    Task<IEnumerable<Flight>> GetAllAsync();

    Task<IEnumerable<Flight>> GetMatchingCriteriaAsync(FlightSearchCriteria criteria);

    Task<Flight?> GetByIdAsync(int id);
    
    Task AddAsync(IEnumerable<Flight> flights);
} //End of IFlightRepository interface
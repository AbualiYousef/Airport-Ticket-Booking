using BusinessLogic.DTOs;
using DataAccess.SearchCriteria;

namespace BusinessLogic.Services.Interfaces;

public interface IFlightService
{
    Task<FlightDto> GetByIdAsync(Guid id);
    Task<IEnumerable<FlightDto>> GetAllAsync();
    Task<IEnumerable<FlightDto>> GetMatchingCriteriaAsync(FlightSearchCriteria criteria);
    // Task<string> ImportFlightsFromCsvAsync(string pathToCsv);
    
}
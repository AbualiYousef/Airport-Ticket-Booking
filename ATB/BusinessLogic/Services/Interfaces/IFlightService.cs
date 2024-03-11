using BusinessLogic.DTOs;
using DataAccess.SearchCriteria;

namespace BusinessLogic.Services.Interfaces;

public interface IFlightService
{
    Task<FlightDto> GetByIdAsync(Guid id);
    Task<IEnumerable<FlightDto>> GetAllAsync();
    Task<IEnumerable<FlightDto>> GetAvailableFlightsMatchingCriteria(FlightSearchCriteria criteria);
    Task<List<string>> ImportFlightsFromCsvAsync(string filePath);
}
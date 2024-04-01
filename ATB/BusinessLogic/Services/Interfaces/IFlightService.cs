using BusinessLogic.DTOs;
using DataAccess.SearchCriteria;

namespace BusinessLogic.Services.Interfaces;

public interface IFlightService
{
    Task<FlightDto> GetByIdAsync(Guid id);
    Task<List<FlightDto>> GetAllAsync();
    Task<List<FlightDto>> GetAvailableFlightsMatchingCriteria(FlightSearchCriteria criteria);
    Task<List<string>> ImportFlightsFromCsvAsync(string filePath);
}
using DataAccess.CsvHelperService;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;

namespace DataAccess.Repositories.Implementations;

public class CsvFlightRepository(ICsvFileService<Flight> csvFileService, string pathToCsv) : IFlightRepository
{
    private List<Flight> _flightsCache = null!;
    private bool _isCacheInitialized = false;

    private async Task InitializeCacheAsync()
    {
        if (!_isCacheInitialized)
        {
            _flightsCache = await csvFileService.ReadFromCsvAsync(pathToCsv);
            _isCacheInitialized = true;
        }
    }

    public async Task<IEnumerable<Flight>> GetAllAsync()
    {
        await InitializeCacheAsync();
        return _flightsCache;
    }

    public async Task<IEnumerable<Flight>> GetMatchingCriteriaAsync(FlightSearchCriteria criteria)
    {
        await InitializeCacheAsync();
        return _flightsCache.Where(criteria.Matches);
    }

    public async Task<Flight?> GetByIdAsync(int id)
    {
        await InitializeCacheAsync();
        return _flightsCache.FirstOrDefault(f => f.Id == id);
    }

    public async Task AddAsync(IEnumerable<Flight> flights)
    {
        await InitializeCacheAsync();
        _flightsCache.AddRange(flights);
        await csvFileService.WriteToCsvAsync(pathToCsv, _flightsCache);
    }
} //End of CsvIFlightRepository class
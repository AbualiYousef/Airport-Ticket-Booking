using DataAccess.CsvHelperService;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;

namespace DataAccess.Repositories.Implementations;

public class CsvIFlightRepository : IFlightRepository
{
    private readonly string _pathToCsv;
    private readonly ICsvFileService<Flight> _csvFileService;
    private List<Flight> _flightsCache;
    private bool _isCacheInitialized = false;

    public CsvIFlightRepository(ICsvFileService<Flight> csvFileService, string pathToCsv)
    {
        _csvFileService = csvFileService;
        _pathToCsv = pathToCsv;
    }

    private async Task InitializeCacheAsync()
    {
        if (!_isCacheInitialized)
        {
            _flightsCache = await _csvFileService.ReadFromCsvAsync(_pathToCsv);
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
        await _csvFileService.WriteToCsvAsync(_pathToCsv, _flightsCache);
    }
} //End of CsvIFlightRepository class
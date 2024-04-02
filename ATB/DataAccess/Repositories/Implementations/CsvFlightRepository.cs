using DataAccess.CsvHelperService;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;
using System.Collections.Concurrent;

namespace DataAccess.Repositories.Implementations;

public class CsvFlightRepository : IFlightRepository
{
    private readonly string _pathToCsv;
    private readonly ICsvFileService<Flight> _csvFileService;
    private ConcurrentDictionary<Guid, Flight> _flightsCache;
    
    public CsvFlightRepository(ICsvFileService<Flight> csvFileService, string pathToCsv)
    {
        _csvFileService = csvFileService;
        _pathToCsv = pathToCsv;
        InitializeCacheAsync().Wait(); 
    }
    public static async Task<CsvFlightRepository> CreateAsync(ICsvFileService<Flight> csvFileService, string pathToCsv)
    {
        var repository = new CsvFlightRepository(csvFileService, pathToCsv);
        await repository.InitializeCacheAsync();
        return repository;
    }

    private async Task InitializeCacheAsync()
    {
        var flights = await _csvFileService.ReadFromCsvAsync(_pathToCsv);
        _flightsCache = new ConcurrentDictionary<Guid, Flight>(flights.ToDictionary(f => f.Id));
    }

    public Task<List<Flight>> GetAllAsync()
    {
        return Task.FromResult(_flightsCache.Values.ToList());
    }

    public Task<Flight?> GetByIdAsync(Guid id)
    {
        _flightsCache.TryGetValue(id, out var flight);
        return Task.FromResult(flight);
    }

    public async Task AddAsync(IEnumerable<Flight> flights)
    {
        var flightsList = flights.ToList();
        var updatedFlights = _flightsCache.Values.ToList();
        updatedFlights.AddRange(flightsList);
        await _csvFileService.WriteToCsvAsync(_pathToCsv, updatedFlights);

        foreach (var flight in flightsList)
        {
            _flightsCache.TryAdd(flight.Id, flight);
        }
    }

    public Task<List<Flight>> GetMatchingCriteriaAsync(FlightSearchCriteria criteria)
    {
        var matchingFlights = _flightsCache.Values.Where(criteria.Matches).ToList();
        return Task.FromResult(matchingFlights);
    }
}
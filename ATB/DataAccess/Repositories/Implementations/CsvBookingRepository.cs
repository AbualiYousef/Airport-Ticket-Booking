using DataAccess.CsvHelperService;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;

namespace DataAccess.Repositories.Implementations;

public class CsvBookingRepository : IBookingRepository
{
    private readonly string _pathToCsv;
    private readonly ICsvFileService<Booking> _csvFileService;
    private List<Booking> _bookingsCache;
    private bool _isCacheInitialized = false;

    public CsvBookingRepository(ICsvFileService<Booking> csvFileService, string pathToCsv)
    {
        _csvFileService = csvFileService;
        _pathToCsv = pathToCsv;
    }

    private async Task InitializeCacheAsync()
    {
        if (!_isCacheInitialized)
        {
            _bookingsCache = await _csvFileService.ReadFromCsvAsync(_pathToCsv);
            _isCacheInitialized = true;
        }
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        await InitializeCacheAsync();
        return _bookingsCache;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        await InitializeCacheAsync();
        return _bookingsCache.FirstOrDefault(b => b.Id == id);
    }

    public async Task AddAsync(Booking booking)
    {
        await InitializeCacheAsync();
        _bookingsCache.Add(booking);
        await _csvFileService.WriteToCsvAsync(_pathToCsv, _bookingsCache);
    }

    public async Task UpdateAsync(Booking booking)
    {
        await InitializeCacheAsync();
        var index = _bookingsCache.FindIndex(b => b.Id == booking.Id);
        if (index != -1)
        {
            _bookingsCache[index] = booking;
            await _csvFileService.WriteToCsvAsync(_pathToCsv, _bookingsCache);
        }
    }

    public async Task DeleteAsync(Booking booking)
    {
        await InitializeCacheAsync();
        _bookingsCache.RemoveAll(b => b.Id == booking.Id);
        await _csvFileService.WriteToCsvAsync(_pathToCsv, _bookingsCache);
    }

    public async Task<IEnumerable<Booking>> GetPassengerBookingsAsync(Guid passengerId)
    {
        await InitializeCacheAsync();
        return _bookingsCache.Where(b => b.Passenger.Id == passengerId);
    }

    public async Task<IEnumerable<Booking>> GetBookingsForFlightWithClassAsync(Guid flightId, FlightClass flightClass)
    {
        await InitializeCacheAsync();
        return _bookingsCache.Where(b => b.Flight.Id == flightId && b.BookingClass == flightClass);
    }

    public async Task<IEnumerable<Booking>> GetMatchingCriteriaAsync(BookingSearchCriteria criteria)
    {
        await InitializeCacheAsync();
        return _bookingsCache.Where(criteria.Matches);
    }
}//End of CsvBookingRepository class

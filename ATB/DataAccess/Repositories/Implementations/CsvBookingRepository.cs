using DataAccess.CsvHelperService;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;
using System.Collections.Concurrent;

namespace DataAccess.Repositories.Implementations;

public class CsvBookingRepository : IBookingRepository
{
    private readonly string _pathToCsv;
    private readonly ICsvFileService<Booking> _csvFileService;
    private ConcurrentDictionary<Guid, Booking> _bookingsCache;

    public CsvBookingRepository(ICsvFileService<Booking> csvFileService, string pathToCsv)
    {
        _csvFileService = csvFileService;
        _pathToCsv = pathToCsv;
    }

    public static async Task<CsvBookingRepository> CreateAsync(ICsvFileService<Booking> csvFileService,
        string pathToCsv)
    {
        var repository = new CsvBookingRepository(csvFileService, pathToCsv);
        await repository.InitializeCacheAsync();
        return repository;
    }

    private async Task InitializeCacheAsync()
    {
        var bookings = await _csvFileService.ReadFromCsvAsync(_pathToCsv);
        _bookingsCache = new ConcurrentDictionary<Guid, Booking>(bookings.ToDictionary(b => b.Id));
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await Task.FromResult(_bookingsCache.Values.ToList());
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        _bookingsCache.TryGetValue(id, out var booking);
        return await Task.FromResult(booking);
    }

    public async Task AddAsync(Booking booking)
    {
        var updatedBookings = _bookingsCache.Values.ToList();
        updatedBookings.Add(booking);
        await _csvFileService.WriteToCsvAsync(_pathToCsv, updatedBookings);
        _bookingsCache.TryAdd(booking.Id, booking);
    }

    public async Task UpdateAsync(Booking booking)
    {
        var updatedBookings = _bookingsCache.Values.ToList();
        var index = updatedBookings.FindIndex(b => b.Id == booking.Id);
        if (index != -1)
        {
            updatedBookings[index] = booking;
            await _csvFileService.WriteToCsvAsync(_pathToCsv, updatedBookings);
            _bookingsCache[booking.Id] = booking;
        }
    }

    public async Task DeleteAsync(Booking booking)
    {
        var updatedBookings = _bookingsCache.Values.ToList();
        updatedBookings.RemoveAll(b => b.Id == booking.Id);
        await _csvFileService.WriteToCsvAsync(_pathToCsv, updatedBookings);
        _bookingsCache.TryRemove(booking.Id, out _);
    }

    public async Task<List<Booking>> GetPassengerBookingsAsync(Guid passengerId)
    {
        var bookings =
            _bookingsCache
                .Values
                .Where(booking => booking.Passenger.Id == passengerId).ToList();
        return await Task.FromResult(bookings);
    }

    public async Task<List<Booking>> GetBookingsForFlightWithClassAsync(Guid flightId, FlightClass flightClass)
    {
        var bookings =
            _bookingsCache
                .Values
                .Where(b => b.Flight.Id == flightId && b.BookingClass == flightClass).ToList();
        return await Task.FromResult(bookings);
    }

    public async Task<List<Booking>> GetMatchingCriteriaAsync(BookingSearchCriteria criteria)
    {
        var bookings = _bookingsCache.Values.Where(criteria.Matches).ToList();
        return await Task.FromResult(bookings);
    }
}
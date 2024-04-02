using AutoFixture;
using AutoFixture.AutoMoq;
using DataAccess.CsvHelperService;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.Implementations;
using DataAccess.SearchCriteria;
using FluentAssertions;
using Moq;

namespace ATB.Tests.Repositories;

public class CsvBookingRepositoryTests
{
    private readonly Mock<ICsvFileService<Booking>> _mockCsvFileService;
    private readonly IFixture _fixture;
    private readonly string _pathToCsv = Path.GetTempPath() + "bookings.csv";
    private List<Booking> _bookingsDataCache;
    private CsvBookingRepository _repository;

    public CsvBookingRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<Flight>(c => c
            .With(f => f.DepartureDate, GetRandomBookingDate())
            .With(f => f.ClassDetails, _fixture.CreateMany<FlightClassDetails>(3).ToList())
        );
        _fixture.Customize<Booking>(c => c
            .With(b => b.Flight, _fixture.Create<Flight>())
            .With(b => b.Passenger, _fixture.Create<Passenger>())
            .With(b => b.BookingDate, GetRandomBookingDate())
        );
        _mockCsvFileService = _fixture.Freeze<Mock<ICsvFileService<Booking>>>();
        _bookingsDataCache = _fixture.CreateMany<Booking>(10).ToList();
        _mockCsvFileService.Setup(svc => svc.ReadFromCsvAsync(_pathToCsv))
            .ReturnsAsync(_bookingsDataCache)
            .Verifiable();

        _repository = new CsvBookingRepository(_mockCsvFileService.Object, _pathToCsv);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBookings()
    {
        var result = await _repository.GetAllAsync();
        result.Should().BeEquivalentTo(_bookingsDataCache);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBooking_WhenBookingExists()
    {
        var expectedBooking = _bookingsDataCache.First();
        var result = await _repository.GetByIdAsync(expectedBooking.Id);
        result.Should().BeEquivalentTo(expectedBooking);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBookingDoesNotExist()
    {
        var nonExistentId = _fixture.Create<Guid>();
        var result = await _repository.GetByIdAsync(nonExistentId);
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_AddsBooking_Correctly()
    {
        var newBooking = _fixture.Create<Booking>();
        await _repository.AddAsync(newBooking);
        var result = await _repository.GetAllAsync();
        result.Should().Contain(newBooking);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBooking()
    {
        var bookingToUpdate = _bookingsDataCache.First();
        bookingToUpdate.Passenger.Name = "Updated Name";
        await _repository.UpdateAsync(bookingToUpdate);
        var updatedBooking = await _repository.GetByIdAsync(bookingToUpdate.Id);
        updatedBooking.Should().BeEquivalentTo(bookingToUpdate);
    }

    [Fact]
    public async Task DeleteAsync_DeletesBooking()
    {
        var bookingToDelete = _bookingsDataCache.First();
        await _repository.DeleteAsync(bookingToDelete);
        var result = await _repository.GetAllAsync();
        result.Should().NotContain(bookingToDelete);
    }

    [Fact]
    public async Task GetPassengerBookingsAsync_ReturnsBookingsForPassenger()
    {
        var passengerId = _bookingsDataCache.First().Passenger.Id;
        var result = await _repository.GetPassengerBookingsAsync(passengerId);
        result.Should().OnlyContain(booking => booking.Passenger.Id == passengerId);
    }

    [Fact]
    public async Task GetBookingsForFlightWithClassAsync_ReturnsBookingsForFlightAndClass()
    {
        var flightId = _bookingsDataCache.First().Flight.Id;
        var flightClass = _bookingsDataCache.First().BookingClass;
        var result = await _repository.GetBookingsForFlightWithClassAsync(flightId, flightClass);
        result.Should().OnlyContain(booking => booking.Flight.Id == flightId && booking.BookingClass == flightClass);
    }

    [Fact]
    public async Task GetMatchingCriteriaAsync_ReturnsMatchingBookings()
    {
        var criteria = new BookingSearchCriteria
        {
            PassengerId = _bookingsDataCache.First().Passenger.Id,
            FlightId = _bookingsDataCache.First().Flight.Id,
            Price = _bookingsDataCache.First().Flight.ClassDetails.First().Price,
            DepartureCountry = _bookingsDataCache.First().Flight.DepartureCountry,
            DestinationCountry = _bookingsDataCache.First().Flight.DestinationCountry,
            DepartureDate = _bookingsDataCache.First().Flight.DepartureDate,
            DepartureAirport = _bookingsDataCache.First().Flight.DepartureAirport,
            ArrivalAirport = _bookingsDataCache.First().Flight.ArrivalAirport,
            Class = _bookingsDataCache.First().BookingClass
        };

        var expectedBookings = _bookingsDataCache.Where(b =>
            b.Passenger.Id == criteria.PassengerId &&
            b.Flight.Id == criteria.FlightId &&
            b.Flight.ClassDetails.Any(details => details.Class == b.BookingClass && details.Price == criteria.Price) &&
            b.Flight.DepartureCountry == criteria.DepartureCountry &&
            b.Flight.DestinationCountry == criteria.DestinationCountry &&
            b.Flight.DepartureDate == criteria.DepartureDate &&
            b.Flight.DepartureAirport == criteria.DepartureAirport &&
            b.Flight.ArrivalAirport == criteria.ArrivalAirport &&
            b.BookingClass == criteria.Class
        ).ToList();

        var result = await _repository.GetMatchingCriteriaAsync(criteria);

        result.Should().BeEquivalentTo(expectedBookings);
    }

    [Fact]
    public async Task GetMatchingCriteriaAsync_ReturnsAllBookings_WhenCriteriaIsNull()
    {
        var result = await _repository.GetMatchingCriteriaAsync(null);
        result.Should().BeEquivalentTo(_bookingsDataCache);
    }

    [Fact]
    public async Task GetMatchingCriteriaAsync_ReturnsAllBookings_WhenCriteriaIsEmpty()
    {
        var criteria = new BookingSearchCriteria();
        var result = await _repository.GetMatchingCriteriaAsync(criteria);
        result.Should().BeEquivalentTo(_bookingsDataCache);
    }

    [Fact]
    public async Task GetMatchingCriteriaAsync_ReturnsEmptyList_WhenCriteriaDoesNotMatchAnyBookings()
    {
        var criteria = new BookingSearchCriteria
        {
            PassengerId = Guid.NewGuid(),
            FlightId = Guid.NewGuid(),
            Price = 100,
            DepartureCountry = "USA",
            DestinationCountry = "UK",
            DepartureDate = DateTime.Now,
            DepartureAirport = "JFK",
            ArrivalAirport = "LHR",
            Class = FlightClass.Business
        };
        var result = await _repository.GetMatchingCriteriaAsync(criteria);
        result.Should().BeEmpty();
    }

    private DateTime GetRandomBookingDate()
    {
        var startDate = new DateTime(2020, 3, 8);
        var endDate = new DateTime(2025, 3, 8);
        var timeSpan = endDate - startDate;
        var randomTest = new Random();
        var newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
        return startDate + newSpan;
    }
}
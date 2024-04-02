using Moq;
using AutoFixture;
using AutoFixture.AutoMoq;
using BusinessLogic.Services.Implementations;
using DataAccess.Repositories.Interfaces;
using DataAccess.CsvHelperService;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.SearchCriteria;

public class FlightServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IFlightRepository> _mockFlightRepository;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<ICsvFileService<Flight>> _mockCsvFileService;
    private readonly FlightService _flightService;

    public FlightServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<Flight>(c
            => c.With(f => f.DepartureDate, GetRandomDate()));
        _mockFlightRepository = _fixture.Freeze<Mock<IFlightRepository>>();
        _mockBookingRepository = _fixture.Freeze<Mock<IBookingRepository>>();
        _mockCsvFileService = _fixture.Freeze<Mock<ICsvFileService<Flight>>>();
        _flightService = new FlightService(_mockFlightRepository.Object, _mockBookingRepository.Object,
            _mockCsvFileService.Object);
    }

    [Fact]
    public async Task GetByIdAsync_FlightExists_ReturnsFlightDto()
    {
        var flightId = Guid.NewGuid();
        var flight = _fixture.Build<Flight>()
            .With(f => f.Id, flightId)
            .With(f => f.DepartureDate, GetRandomDate())
            .Create();
        _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId)).ReturnsAsync(flight);

        var result = await _flightService.GetByIdAsync(flightId);

        Assert.NotNull(result);
        Assert.Equal(flightId, result.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlightDtos()
    {
        var flights = _fixture.CreateMany<Flight>(5).ToList();
        _mockFlightRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(flights);

        var result = await _flightService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(flights.Count, result.Count);
        Assert.Equal(flights.Select(f => f.Id), result.Select(f => f.Id));
    }

    [Fact]
    public async Task GetAvailableFlightsMatchingCriteria_NoClassCriteria_ReturnsAvailableFlights()
    {
        var criteria = new FlightSearchCriteria();
        var flights = _fixture.CreateMany<Flight>(5).ToList();
        _mockFlightRepository.Setup(repo
            => repo.GetMatchingCriteriaAsync(criteria)).ReturnsAsync(flights);

        var result = await _flightService.GetAvailableFlightsMatchingCriteria(criteria);

        Assert.NotNull(result);
        Assert.Equal(flights.Count, result.Count);
    }

    [Fact]
    public async Task GetAvailableFlightsMatchingCriteria_SpecificClassCriteria_ReturnsAvailableFlights()
    {
        var criteria = new FlightSearchCriteria { Class = FlightClass.Business };
        var flights = _fixture.CreateMany<Flight>(5).ToList();
        _mockFlightRepository.Setup(repo
            => repo.GetMatchingCriteriaAsync(criteria)).ReturnsAsync(flights);

        var result = await _flightService.GetAvailableFlightsMatchingCriteria(criteria);

        Assert.NotEmpty(result);
        Assert.Equal(flights.Count, result.Count);
    }

    [Fact]
    public async Task ImportFlightsFromCsvAsync_AllValidFlights_ImportsSuccessfully_Revised()
    {
        var validFlights = _fixture.Build<Flight>()
            .With(f => f.DepartureDate, DateTime.Now.AddDays(1))
            .CreateMany(5).ToList();
        _mockCsvFileService.Setup(s => s.ReadFromCsvAsync(It.IsAny<string>())).ReturnsAsync(validFlights);

        var result = await _flightService.ImportFlightsFromCsvAsync("valid_flights.csv");

        Assert.Empty(result);
        _mockFlightRepository.Verify(r => r.AddAsync(It.Is<List<Flight>>(flights => flights.Count == 5)), Times.Once);
    }

    [Fact]
    public async Task ImportFlightsFromCsvAsync_SomeInvalidFlights_ReturnsValidationErrors()
    {
        var validFlights = _fixture.Build<Flight>()
            .With(f => f.DepartureDate, DateTime.Now.AddDays(1))
            .CreateMany(5).ToList();
        var invalidFlights = _fixture.Build<Flight>()
            .Without(f => f.DepartureDate)
            .CreateMany(2).ToList();
        var allFlights = validFlights.Concat(invalidFlights).ToList();
        _mockCsvFileService
            .Setup(s => s
                .ReadFromCsvAsync(It.IsAny<string>())).ReturnsAsync(allFlights);

        var result = await _flightService.ImportFlightsFromCsvAsync("invalid_flights.csv");

        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        _mockFlightRepository
            .Verify(r => r
                .AddAsync(It.Is<List<Flight>>(flights => flights.Count == 5)), Times.Once);
    }

    private DateTime GetRandomDate()
    {
        var startDate = new DateTime(2020, 3, 8);
        var endDate = new DateTime(2025, 3, 8);
        var timeSpan = endDate - startDate;
        var randomTest = new Random();
        var newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
        return startDate + newSpan;
    }
}
using AutoFixture;
using AutoFixture.AutoMoq;
using DataAccess.CsvHelperService;
using DataAccess.Models;
using DataAccess.Repositories.Implementations;
using DataAccess.SearchCriteria;
using FluentAssertions;
using Moq;

namespace ATB.Tests;

public class CsvFlightRepositoryTests
{
    private readonly Mock<ICsvFileService<Flight>> _mockCsvFileService;
    private readonly IFixture _fixture;
    private readonly string _pathToCsv = Path.GetTempPath() + "flights.csv";
    private List<Flight> _flightsDataCache;
    private CsvFlightRepository _repository;

    public CsvFlightRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<Flight>(c
            => c.With(f => f.DepartureDate, GetRandomDepartureDate()));
        _mockCsvFileService = _fixture.Freeze<Mock<ICsvFileService<Flight>>>();
        _flightsDataCache = _fixture.CreateMany<Flight>(10).ToList();

        _mockCsvFileService.Setup(svc => svc.ReadFromCsvAsync(_pathToCsv))
            .ReturnsAsync(_flightsDataCache)
            .Verifiable();

        _repository = new CsvFlightRepository(_mockCsvFileService.Object, _pathToCsv);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlights()
    {
        var result = await _repository.GetAllAsync();
        result.Should().BeEquivalentTo(_flightsDataCache);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFlight_WhenFlightExists()
    {
        var expectedFlight = _flightsDataCache.First();
        var result = await _repository.GetByIdAsync(expectedFlight.Id);
        result.Should().BeEquivalentTo(expectedFlight);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        var nonExistentId = Guid.NewGuid();

        var flight = await _repository.GetByIdAsync(nonExistentId);

        flight.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_AddsFlightsCorrectly()
    {
        var newFlights = _fixture.CreateMany<Flight>(5).ToList();
        _mockCsvFileService.Setup(svc
                => svc.WriteToCsvAsync(_pathToCsv, It.IsAny<IEnumerable<Flight>>()))
            .Callback<string, IEnumerable<Flight>>((path, flights)
                =>
            {
                _flightsDataCache.AddRange(flights);
            })
            .Returns(Task.CompletedTask)
            .Verifiable();

        await _repository.AddAsync(newFlights);

        _mockCsvFileService.Verify();
        foreach (var flight in newFlights)
        {
            var retrievedFlight = await _repository.GetByIdAsync(flight.Id);
            retrievedFlight.Should().BeEquivalentTo(flight);
        }
    }

    [Fact]
    public async Task AddAsync_WithEmptyFlightsList_DoesNotChangeFlights()
    {
        await _repository.AddAsync(new List<Flight>());
        var flights = await _repository.GetAllAsync();
        flights.Should().BeEquivalentTo(_flightsDataCache);
    }

    [Fact]
    public async Task GetMatchingCriteriaAsync_ReturnsMatchingFlights()
    {
        var criteria = _fixture.Create<FlightSearchCriteria>();
        var matchingFlights = _flightsDataCache
            .Where(f => f.DepartureCountry == criteria.DepartureCountry).ToList();
        var result = await _repository.GetMatchingCriteriaAsync(criteria);
        result.Should().BeEquivalentTo(matchingFlights);
    }

    [Fact]
    public async Task GetMatchingCriteriaAsync_WithNonMatchingCriteria_ReturnsNoFlights()
    {
        var criteria = _fixture.Build<FlightSearchCriteria>()
            .With(c => c.DepartureCountry, "NonExistentDepartureCountry")
            .Create();
        var result = await _repository.GetMatchingCriteriaAsync(criteria);
        result.Should().BeEmpty();
    }


    [Fact]
    public async Task GetMatchingCriteriaAsync_WithEmptyCriteria_ReturnsAllFlights()
    {
        var criteria = new FlightSearchCriteria();
        var result = await _repository.GetMatchingCriteriaAsync(criteria);
        result.Should().BeEquivalentTo(_flightsDataCache);
    }

    private DateTime GetRandomDepartureDate()
    {
        var startDate = new DateTime(2020, 3, 8);
        var endDate = new DateTime(2025, 3, 8);
        var range = (endDate - startDate).Days;

        return startDate.AddDays(_fixture.Create<int>() % range);
    }
}
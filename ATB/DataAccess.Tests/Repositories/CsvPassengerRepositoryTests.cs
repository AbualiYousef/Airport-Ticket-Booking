using AutoFixture;
using AutoFixture.AutoMoq;
using DataAccess.CsvHelperService;
using DataAccess.Models;
using DataAccess.Repositories.Implementations;
using FluentAssertions;
using Moq;

namespace ATB.Tests;

public class CsvPassengerRepositoryTests
{
    private readonly Mock<ICsvFileService<Passenger>> _mockCsvFileService;
    private readonly IFixture _fixture;
    private readonly string _pathToCsv = Path.GetTempPath() + "passengers.csv";
    private List<Passenger> _passengersDataCache;
    private CsvPassengerRepository _repository;

    public CsvPassengerRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<Passenger>(c => c
            .With(p => p.Id, Guid.NewGuid())
        );
        _mockCsvFileService = _fixture.Freeze<Mock<ICsvFileService<Passenger>>>();
        _passengersDataCache = _fixture.CreateMany<Passenger>(10).ToList();

        _mockCsvFileService.Setup(svc => svc.ReadFromCsvAsync(_pathToCsv))
            .ReturnsAsync(_passengersDataCache)
            .Verifiable();

        _repository = new CsvPassengerRepository(_mockCsvFileService.Object, _pathToCsv);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPassenger_WhenPassengerExists()
    {
        var expectedPassenger = _passengersDataCache.First();
        var result = await _repository.GetByIdAsync(expectedPassenger.Id);
        result.Should().BeEquivalentTo(expectedPassenger);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenPassengerDoesNotExist()
    { 
        var nonExistentId = Guid.NewGuid();
        var result = await _repository.GetByIdAsync(nonExistentId);
        result.Should().BeNull();
    }
}
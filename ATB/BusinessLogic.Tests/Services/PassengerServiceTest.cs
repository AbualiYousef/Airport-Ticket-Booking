using AutoFixture;
using AutoFixture.AutoMoq;
using BusinessLogic.Services.Implementations;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using Moq;

namespace BusinessLogic.Tests.Services
{
    public class PassengerServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IPassengerRepository> _mockPassengerRepository;
        private readonly PassengerService _passengerService;

        public PassengerServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockPassengerRepository = _fixture.Freeze<Mock<IPassengerRepository>>();
            _passengerService = new PassengerService(_mockPassengerRepository.Object);
        }

        [Fact]
        public async Task GetById_PassengerDoesNotExist_ReturnsNull()
        {
            var nonExistentId = Guid.NewGuid();
            _mockPassengerRepository
                .Setup(repo => repo.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Passenger)null);

            var result = await _passengerService.GetById(nonExistentId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetById_PassengerExists_ReturnsPassengerDto()
        {
            var existingId = Guid.NewGuid();
            var passenger = _fixture.Create<Passenger>();
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(existingId))
                .ReturnsAsync(passenger);

            var result = await _passengerService.GetById(existingId);

            Assert.NotNull(result);
            Assert.Equal(passenger.Id, result.Id); 
            Assert.Equal(passenger.Name, result.Name);
            Assert.Equal(passenger.Email, result.Email);
            Assert.Equal(passenger.PhoneNumber, result.PhoneNumber);
            Assert.Equal(passenger.PassportNumber, result.PassportNumber);
        }
    }
}
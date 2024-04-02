using AutoFixture;
using AutoFixture.AutoMoq;
using BusinessLogic.DTOs;
using BusinessLogic.Services.Implementations;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using DataAccess.SearchCriteria;
using FluentAssertions;
using Moq;


namespace BusinessLogic.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IFlightRepository> _mockFlightRepository;
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IPassengerRepository> _mockPassengerRepository;
        private readonly BookingService _bookingService;
        private readonly Flight _flight;
        private readonly Booking _booking;
        private readonly Passenger _passenger;

        public BookingServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customize<Booking>(c
                => c.With(b => b.BookingDate, GetRandomDate()));
            _fixture.Customize<Flight>(c
                => c.With(f => f.DepartureDate, GetRandomDate()));
            _mockFlightRepository = _fixture.Freeze<Mock<IFlightRepository>>();
            _mockBookingRepository = _fixture.Freeze<Mock<IBookingRepository>>();
            _mockPassengerRepository = _fixture.Freeze<Mock<IPassengerRepository>>();
            _flight = _fixture.Create<Flight>();
            _booking = _fixture.Create<Booking>();
            _passenger = _fixture.Create<Passenger>();
            _bookingService = new BookingService(_mockBookingRepository.Object, _mockFlightRepository.Object,
                _mockPassengerRepository.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenBookingDoesNotExist()
        {
            var bookingId = Guid.NewGuid();
            _mockBookingRepository.Setup(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync((Booking)null);

            var result = await _bookingService.GetByIdAsync(bookingId);

            result.Should().BeNull();
        }


        [Fact]
        public async Task GetByIdAsync_ReturnsBookingDto_WhenBookingExists()
        {
            var bookingId = Guid.NewGuid();
            var booking = _fixture.Build<Booking>()
                .With(b => b.Id, bookingId)
                .With(b => b.BookingDate, GetRandomDate())
                .Create();
            _mockBookingRepository.Setup(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);

            var result = await _bookingService.GetByIdAsync(bookingId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(bookingId);
        }

        [Fact]
        public async Task BookFlight_Successfully_WhenConditionsAreMet()
        {
            var flightId = _flight.Id;
            var passengerId = _passenger.Id;

            _flight.ClassDetails = new List<FlightClassDetails>
            {
                new FlightClassDetails { Class = FlightClass.Economy, Capacity = 20 }
            };

            _mockFlightRepository
                .Setup(repo => repo
                    .GetByIdAsync(flightId))
                .ReturnsAsync(_flight);
            _mockPassengerRepository
                .Setup(repo => repo
                    .GetByIdAsync(passengerId))
                .ReturnsAsync(_passenger);
            _mockBookingRepository
                .Setup(repo => repo
                    .GetBookingsForFlightWithClassAsync(flightId, FlightClass.Economy))
                .ReturnsAsync(new List<Booking>());
            _mockBookingRepository
                .Setup(r => r
                    .AddAsync(It.IsAny<Booking>()))
                .Returns(Task.CompletedTask);

            Func<Task> bookFlight = async () =>
                await _bookingService.BookFlight(flightId, passengerId, FlightClass.Economy);

            await bookFlight.Should().NotThrowAsync();
            _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task BookFlight_ThrowsArgumentException_WhenFlightNotFound()
        {
            var flightId = Guid.NewGuid();
            var passengerId = _passenger.Id;

            _mockFlightRepository
                .Setup(repo => repo
                    .GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            Func<Task> bookFlight = async () =>
                await _bookingService.BookFlight(flightId, passengerId, FlightClass.Economy);

            await bookFlight.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"Flight with id {flightId} not found");
        }

        [Fact]
        public async Task CancelBooking_Successfully_WhenConditionsAreMet()
        {
            var bookingId = _booking.Id;

            _mockBookingRepository
                .Setup(repo => repo
                    .GetByIdAsync(bookingId))
                .ReturnsAsync(_booking);
            _mockBookingRepository
                .Setup(r => r
                    .DeleteAsync(It.IsAny<Booking>()))
                .Returns(Task.CompletedTask);

            Func<Task> cancelBooking = async () =>
                await _bookingService.CancelBooking(bookingId);

            await cancelBooking.Should().NotThrowAsync();
            _mockBookingRepository.Verify(r => r.DeleteAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task CancelBooking_ThrowsArgumentException_WhenBookingNotFound()
        {
            var bookingId = Guid.NewGuid();

            _mockBookingRepository
                .Setup(repo => repo
                    .GetByIdAsync(bookingId))
                .ReturnsAsync((Booking)null);

            Func<Task> cancelBooking = async () =>
                await _bookingService.CancelBooking(bookingId);

            await cancelBooking.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"Booking with id {bookingId} not found");
        }

        [Fact]
        public async Task ModifyBooking_Successfully_WhenConditionsAreMet()
        {
            var bookingId = _booking.Id;
            var newClass = _fixture.Create<FlightClass>();

            _mockBookingRepository
                .Setup(repo => repo
                    .GetByIdAsync(bookingId))
                .ReturnsAsync(_booking);
            _mockBookingRepository
                .Setup(r => r
                    .UpdateAsync(It.IsAny<Booking>()))
                .Returns(Task.CompletedTask);

            Func<Task> modifyBooking = async () =>
                await _bookingService.ModifyBooking(bookingId, newClass);

            await modifyBooking.Should().NotThrowAsync();
            _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task ModifyBooking_ThrowsArgumentException_WhenBookingNotFound()
        {
            var bookingId = Guid.NewGuid();
            var newClass = _fixture.Create<FlightClass>();

            _mockBookingRepository
                .Setup(repo => repo
                    .GetByIdAsync(bookingId))
                .ReturnsAsync((Booking)null);

            Func<Task> modifyBooking = async () =>
                await _bookingService.ModifyBooking(bookingId, newClass);

            await modifyBooking.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"Booking with id {bookingId} not found");
        }

        [Fact]
        public async Task GetPassengerBookingsAsync_ThrowsArgumentException_WhenPassengerNotFound()
        {
            var passengerId = Guid.NewGuid();
            _mockPassengerRepository
                .Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync((Passenger)null);

            Func<Task> getPassengerBookings = async () =>
                await _bookingService.GetPassengerBookingsAsync(passengerId);

            await getPassengerBookings.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"Passenger with id {passengerId} not found");
        }

        [Fact]
        public async Task GetPassengerBookingsAsync_ReturnsListOfBookingDtos_WhenPassengerFound()
        {
            var passengerId = _passenger.Id;
            var bookings = _fixture.CreateMany<Booking>().ToList();
            _mockPassengerRepository
                .Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(_passenger);
            _mockBookingRepository
                .Setup(repo => repo.GetPassengerBookingsAsync(passengerId))
                .ReturnsAsync(bookings);

            var result = await _bookingService.GetPassengerBookingsAsync(passengerId);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<BookingDto>>();
            result.Should().HaveCount(bookings.Count);
        }
        
        [Fact]
        public async Task GetMatchingCriteriaAsync_ReturnsListOfBookingDtos_WhenCriteriaIsNull()
        {
            var criteria = new BookingSearchCriteria();
            var bookings = _fixture.CreateMany<Booking>().ToList();
            _mockBookingRepository
                .Setup(repo => repo.GetMatchingCriteriaAsync(criteria))
                .ReturnsAsync(bookings);

            var result = await _bookingService.GetMatchingCriteriaAsync(criteria);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<BookingDto>>();
            result.Should().HaveCount(bookings.Count);
        }
        
        [Fact]
        public async Task GetMatchingCriteriaAsync_ReturnsListOfBookingDtos_WhenCriteriaIsNotNull()
        {
            var criteria = new BookingSearchCriteria
            {
                PassengerId = _passenger.Id
            };
            var bookings = _fixture.CreateMany<Booking>().ToList();
            _mockBookingRepository
                .Setup(repo => repo.GetMatchingCriteriaAsync(criteria))
                .ReturnsAsync(bookings);

            var result = await _bookingService.GetMatchingCriteriaAsync(criteria);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<BookingDto>>();
            result.Should().HaveCount(bookings.Count);
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
}
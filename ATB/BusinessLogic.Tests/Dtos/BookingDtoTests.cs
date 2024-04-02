using BusinessLogic.DTOs;
using DataAccess.Models;
using AutoFixture;
using FluentAssertions;

namespace BusinessLogic.Tests.DTOs
{
    public class BookingDtoTests
    {
        private readonly IFixture _fixture;

        public BookingDtoTests()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(
                new RandomDateTimeSequenceGenerator(new DateTime(2024, 3, 8), new DateTime(2025, 3, 8))
            );
            _fixture.Customize<Flight>(c => c
                .With(f => f.DepartureDate, _fixture.Create<DateTime>())
            );
            _fixture.Customize<Booking>(c => c
                .With(b => b.Flight, _fixture.Create<Flight>())
                .With(b => b.Passenger, _fixture.Create<Passenger>())
                .With(b => b.BookingDate, GetRandomDate())
            );
        }

        [Fact]
        public void BookingDto_CorrectlyInitializesFromBooking()
        {
            var booking = _fixture.Create<Booking>();
            var bookingDto = new BookingDto(booking);
            bookingDto.Id.Should().Be(booking.Id);
            bookingDto.Passenger.Should().BeEquivalentTo(new PassengerDto(booking.Passenger),
                options => options.ComparingByMembers<PassengerDto>());
            bookingDto.Flight.Should().BeEquivalentTo(new FlightDto(booking.Flight),
                options => options.ComparingByMembers<FlightDto>());
            bookingDto.BookingClass.Should().Be(booking.BookingClass);
            bookingDto.BookingDate.Should().Be(booking.BookingDate);
        }

        [Fact]
        public void ToString_ReturnsExpectedFormat()
        {
            var booking = _fixture.Create<Booking>();
            var bookingDto = new BookingDto(booking);
            var expectedString =
                $"Id: {bookingDto.Id}, Passenger: {bookingDto.Passenger}, Flight: {bookingDto.Flight}," +
                $" BookingClass: {bookingDto.BookingClass}, BookingDate: {bookingDto.BookingDate}";
            var toStringOutput = bookingDto.ToString();
            toStringOutput.Should().Be(expectedString);
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
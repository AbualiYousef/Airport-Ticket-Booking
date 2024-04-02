using AutoFixture;
using BusinessLogic.DTOs;
using DataAccess.Enums;
using DataAccess.Models;
using FluentAssertions;

namespace BusinessLogic.Tests.DTOs
{
    public class FlightDtoTests
    {
        private readonly IFixture _fixture;

        public FlightDtoTests()
        {
            _fixture = new Fixture();
            _fixture.Customize<Flight>(c => c
                .With(x => x.DepartureDate, GetRandomDate)
            );
            _fixture.Customize<FlightClassDetails>(c => c
                .With(x => x.Price, _fixture.Create<decimal>())
                .With(x => x.Capacity, _fixture.Create<int>())
                .With(x => x.Class, _fixture.Create<FlightClass>()));
        }

        [Fact]
        public void FlightDto_CorrectlyInitializesFromFlight()
        {
            var flight = _fixture.Create<Flight>();
            var flightDto = new FlightDto(flight);

            flightDto.Id.Should().Be(flight.Id);
            flightDto.DepartureCountry.Should().Be(flight.DepartureCountry);
            flightDto.DestinationCountry.Should().Be(flight.DestinationCountry);
            flightDto.DepartureDate.Should().Be(flight.DepartureDate);
            flightDto.DepartureAirport.Should().Be(flight.DepartureAirport);
            flightDto.ArrivalAirport.Should().Be(flight.ArrivalAirport);
            flightDto.ClassDetails.Should().BeEquivalentTo(
                flight.ClassDetails.Select(details => new FlightClassDetailsDto(details)),
                options => options.ComparingByMembers<FlightClassDetailsDto>());
        }

        [Fact]
        public void ToString_ReturnsExpectedFormat()
        {
            var flight = _fixture.Create<Flight>();
            var flightDto = new FlightDto(flight);

            var expectedString =
                $"Id: {flightDto.Id}, DepartureCountry: {flightDto.DepartureCountry}, " +
                $"DestinationCountry: {flightDto.DestinationCountry}, DepartureDate: {flightDto.DepartureDate}, " +
                $"DepartureAirport: {flightDto.DepartureAirport}, ArrivalAirport: {flightDto.ArrivalAirport}, " +
                $"ClassDetails: {string.Join(", ", flightDto.ClassDetails)}";

            flightDto.ToString().Should().Be(expectedString);
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
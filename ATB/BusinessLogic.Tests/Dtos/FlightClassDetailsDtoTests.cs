using AutoFixture;
using BusinessLogic.DTOs;
using DataAccess.Models;
using FluentAssertions;

namespace BusinessLogic.Tests.DTOs
{
    public class FlightClassDetailsDtoTests
    {
        private readonly IFixture _fixture;

        public FlightClassDetailsDtoTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void FlightClassDetailsDto_CorrectlyInitializesFromFlightClassDetails()
        {
            var flightClassDetails = _fixture.Create<FlightClassDetails>();
            var flightClassDetailsDto = new FlightClassDetailsDto(flightClassDetails);

            flightClassDetailsDto.Class.Should().Be(flightClassDetails.Class);
            flightClassDetailsDto.Price.Should().Be(flightClassDetails.Price);
            flightClassDetailsDto.Capacity.Should().Be(flightClassDetails.Capacity);
        }

        [Fact]
        public void ToString_ReturnsExpectedFormat()
        {
            var flightClassDetails = _fixture.Create<FlightClassDetails>();
            var flightClassDetailsDto = new FlightClassDetailsDto(flightClassDetails);

            var expectedString =
                $"Class: {flightClassDetailsDto.Class}, Price: {flightClassDetailsDto.Price}, Capacity: {flightClassDetailsDto.Capacity}";

            flightClassDetailsDto.ToString().Should().Be(expectedString);
        }
    }
}
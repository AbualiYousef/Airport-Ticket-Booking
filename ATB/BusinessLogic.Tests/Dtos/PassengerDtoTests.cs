using AutoFixture;
using BusinessLogic.DTOs;
using DataAccess.Models;
using FluentAssertions;

namespace BusinessLogic.Tests.DTOs
{
    public class PassengerDtoTests
    {
        private readonly IFixture _fixture;

        public PassengerDtoTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void PassengerDto_CorrectlyInitializesFromPassenger()
        {
            var passenger = _fixture.Create<Passenger>();
            var passengerDto = new PassengerDto(passenger);

            passengerDto.Id.Should().Be(passenger.Id);
            passengerDto.Name.Should().Be(passenger.Name);
            passengerDto.Email.Should().Be(passenger.Email);
            passengerDto.PhoneNumber.Should().Be(passenger.PhoneNumber);
            passengerDto.PassportNumber.Should().Be(passenger.PassportNumber);
        }

        [Fact]
        public void ToString_ReturnsExpectedFormat()
        {
            var passenger = _fixture.Create<Passenger>();
            var passengerDto = new PassengerDto(passenger);

            var expectedString = $"Id: {passengerDto.Id}, Name: {passengerDto.Name}, Email: {passengerDto.Email}, " +
                                 $"PhoneNumber: {passengerDto.PhoneNumber}, PassportNumber: {passengerDto.PassportNumber}";

            passengerDto.ToString().Should().Be(expectedString);
        }
    }
}
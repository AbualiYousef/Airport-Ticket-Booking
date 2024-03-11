using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using DataAccess.Enums;
using DataAccess.SearchCriteria;
using Presentation.Enums;

namespace Presentation;

public class AirportBookingSystem(
    IFlightService flightService,
    IBookingService bookingService)
{
    public async Task RunMainMenu()
    {
        Console.WriteLine("Welcome to the Airport Ticket Booking System");
        Console.WriteLine("Are you a (1) Passenger or (2) Manager?");
        var roleInput = InputParser.GetInput<int>("Select a role: ", int.TryParse);
        var role = Enum.Parse<UserRole>(roleInput.ToString());
        switch (role)
        {
            case UserRole.Passenger:
                await HandlePassengerChoices();
                break;
            case UserRole.Manager:
                await HandleManagerChoices();
                break;
            default:
                Console.WriteLine("Invalid role selected.");
                break;
        }
    }

    #region Handle Choices

    async Task HandlePassengerChoices()
    {
        Console.Clear();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nPassenger Menu:");
            Console.WriteLine("[1] Book a flight.");
            Console.WriteLine("[2] Show available flights.");
            Console.WriteLine("[3] Cancel a booking.");
            Console.WriteLine("[4] Modify a booking");
            Console.WriteLine("[5] View bookings");
            Console.WriteLine("[6] Exit");

            Console.Write("Select an option: ");
            var choiceInput = InputParser.GetInput<int>("Select an option: ", int.TryParse);
            var choice = Enum.Parse<PassengerChoice>(choiceInput.ToString());

            switch (choice)
            {
                case PassengerChoice.BookFlight:
                    await BookFlight();
                    break;
                case PassengerChoice.SearchForFlightsMatchingCriteria:
                    await SearchForFlightsMatchingCriteria();
                    break;
                case PassengerChoice.CancelBooking:
                    await CancelBooking();
                    break;
                case PassengerChoice.ModifyBooking:
                    await ModifyBooking();
                    break;
                case PassengerChoice.ViewBookings:
                    await ViewBookings();
                    break;
                case PassengerChoice.Exit:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    async Task HandleManagerChoices()
    {
        Console.Clear();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nManager Menu:");
            Console.WriteLine("[1] Filter bookings using criteria.");
            Console.WriteLine("[2] Import flights from CSV.");
            Console.WriteLine("[3] Exit.");

            Console.Write("Select an option: ");
            var choiceInput = InputParser.GetInput<int>("Select an option: ", int.TryParse);
            var choice = Enum.Parse<ManagerChoice>(choiceInput.ToString());

            switch (choice)
            {
                case ManagerChoice.FilterBookingUsingCriteria:
                    await FilterBookingUsingCriteria();
                    break;
                case ManagerChoice.ImportFlightsFromCsv:
                    await ImportFlightsFromCsv();
                    break;
                case ManagerChoice.Exit:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    #endregion


    #region Passenger Choices

    async Task BookFlight()
    {
        Console.Clear();
        Console.WriteLine("Book a flight");
        Console.WriteLine("Enter your Flight ID: ");
        var flightId = InputParser.GetInput<Guid>("Flight ID: ", Guid.TryParse);
        var passengerId = InputParser.GetInput<Guid>("Passenger ID: ", Guid.TryParse);
        var flightClass = InputParser.GetInput<FlightClass>("Flight Class: ", Enum.TryParse);
        try
        {
            await bookingService.BookFlight(flightId, passengerId, flightClass);
            Console.WriteLine("Flight booked successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("An error occurred while booking the flight.");
        }
    }

    async Task SearchForFlightsMatchingCriteria()
    {
        Console.Clear();
        Console.WriteLine("Search for Flights Matching Criteria");

        var criteria = new FlightSearchCriteria
        {
            Price = InputParser.GetOptionalInput<decimal>("Price (optional): ", decimal.TryParse),
            DepartureCountry = InputParser.GetOptionalStringInput("Departure Country (optional): "),
            DestinationCountry = InputParser.GetOptionalStringInput("Destination Country (optional): "),
            DepartureDate = InputParser.GetOptionalInput<DateTime>("Departure Date (optional, format: yyyy-MM-dd): ",
                DateTime.TryParse),
            DepartureAirport = InputParser.GetOptionalStringInput("Departure Airport (optional): "),
            ArrivalAirport = InputParser.GetOptionalStringInput("Arrival Airport (optional): "),
            Class = InputParser.GetOptionalInput<FlightClass>("Flight Class (optional): ", Enum.TryParse)
        };
        try
        {
            var flights = await flightService.GetAvailableFlightsMatchingCriteria(criteria);
            var flightDtos = flights.ToList();
            if (flightDtos.Any())
            {
                Console.WriteLine("Matching flights:");
                foreach (var flight in flightDtos)
                {
                    Console.WriteLine(
                        "Hello"
                        // $"Flight ID: {flight.Id}," +
                        // $" Departure Country: {flight.DepartureCountry}," +
                        // $" Destination Country: {flight.DestinationCountry}," +
                        // $" Departure Date: {flight.DepartureDate}," +
                        // $" Departure Airport: {flight.DepartureAirport}," +
                        // $" Arrival Airport: {flight.ArrivalAirport}"
                    );
                }
            }
            else
            {
                Console.WriteLine("No flights match the criteria.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }


    async Task CancelBooking()
    {
        Console.Clear();
        Console.WriteLine("Cancel a booking");
        var bookingId = InputParser.GetInput<Guid>("Booking ID: ", Guid.TryParse);
        try
        {
            await bookingService.CancelBooking(bookingId);
            Console.WriteLine("Booking cancelled successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    async Task ModifyBooking()
    {
        Console.Clear();
        Console.WriteLine("Modify a booking");
        var bookingId = InputParser.GetInput<Guid>("Booking ID: ", Guid.TryParse);
        var newClass = InputParser.GetInput<FlightClass>("New Class: ", Enum.TryParse);
        try
        {
            await bookingService.ModifyBooking(bookingId, newClass);
            Console.WriteLine("Booking modified successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    async Task ViewBookings()
    {
        Console.Clear();
        Console.WriteLine("View bookings");
        var passengerId = InputParser.GetInput<Guid>("Passenger ID: ", Guid.TryParse);
        try
        {
            var bookings = await bookingService.GetPassengerBookingsAsync(passengerId);
            var bookingDtos = bookings.ToList();
            if (bookingDtos.Any())
            {
                Console.WriteLine("Your bookings:");
                foreach (var booking in bookingDtos)
                {
                    Console.WriteLine(
                        $"Booking ID: {booking.Id}," +
                        $" Flight ID: {booking.Flight.Id}," +
                        $" Booking Class: {booking.BookingClass}," +
                        $" Booking Date: {booking.BookingDate}"
                    );
                }
            }
            else
            {
                Console.WriteLine("You have no bookings.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    #endregion

    #region Manager Choices

    async Task FilterBookingUsingCriteria()
    {
        Console.Clear();
        Console.WriteLine("Filter bookings using criteria");
        var criteria = new BookingSearchCriteria
        {
            PassengerId = InputParser.GetOptionalInput<Guid>("Passenger ID (optional): ", Guid.TryParse),
            FlightId = InputParser.GetOptionalInput<Guid>("Flight ID (optional): ", Guid.TryParse),
            Price = InputParser.GetOptionalInput<decimal>("Max Price (optional): ", decimal.TryParse),
            DepartureCountry = InputParser.GetOptionalStringInput("Departure Country (optional): "),
            DestinationCountry = InputParser.GetOptionalStringInput("Destination Country (optional): "),
            DepartureDate = InputParser.GetOptionalInput<DateTime>("Departure Date (optional, format: yyyy-MM-dd): ",
                DateTime.TryParse),
            DepartureAirport = InputParser.GetOptionalStringInput("Departure Airport (optional): "),
            ArrivalAirport = InputParser.GetOptionalStringInput("Arrival Airport (optional): "),
            Class = InputParser.GetOptionalInput<FlightClass>("Flight Class (optional): ", Enum.TryParse)
        };
        try
        {
            var filteredBookings = await bookingService.GetMatchingCriteriaAsync(criteria);

            var bookingDtos = filteredBookings as BookingDto[] ?? filteredBookings.ToArray();
            if (bookingDtos.Any())
            {
                Console.WriteLine("\nFiltered bookings:");
                foreach (var booking in bookingDtos)
                {
                    Console.WriteLine($"Booking ID: {booking.Id}," +
                                      $" Passenger ID: {booking.Passenger.Id}, " +
                                      $"Flight ID: {booking.Flight.Id}," +
                                      $" Price: {booking.Flight.ClassDetails.First().Price}, " +
                                      $"Departure: {booking.Flight.DepartureCountry}" +
                                      $" to {booking.Flight.DestinationCountry}, " +
                                      $"Date: {booking.Flight.DepartureDate}," +
                                      $" Class: {booking.BookingClass}");
                }
            }
            else
            {
                Console.WriteLine("No bookings match the criteria.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while filtering bookings: {e.Message}");
        }
    }

    async Task ImportFlightsFromCsv()
    {
        Console.Clear();
        Console.WriteLine("Import flights from CSV");
        string filePath = InputParser.GetInput("Enter the path to the CSV file: ",
            (string input, out string str) =>
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    str = null!;
                    return false;
                }

                str = input.Trim();
                return true;
            });
        try
        {
            List<string> validationErrors = await flightService.ImportFlightsFromCsvAsync(filePath);

            if (validationErrors.Any())
            {
                Console.WriteLine("Some flights failed to import due to validation errors:");
                foreach (string error in validationErrors)
                {
                    Console.WriteLine($"- {error}");
                }
            }
            else
            {
                Console.WriteLine("All flights imported successfully.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while importing flights: {e.Message}");
        }
    }

    #endregion
}
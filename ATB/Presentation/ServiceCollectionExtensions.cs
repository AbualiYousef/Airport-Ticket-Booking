using BusinessLogic.Services.Implementations;
using BusinessLogic.Services.Interfaces;
using DataAccess.CsvHelperService;
using DataAccess.Models;
using DataAccess.Repositories.Implementations;
using DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IPassengerService, PassengerService>()
            .AddScoped<IFlightService, FlightService>()
            .AddScoped<IBookingService, BookingService>();
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        string pathToPassengerCsv = """D:\\Training\2.C#\Airport Ticket Booking\Airport-Ticket-Booking\ATB\DataAccess\CsvFiles\Passengers.csv""";
        string pathToFlightCsv ="""D:\\Training\2.C#\Airport Ticket Booking\Airport-Ticket-Booking\ATB\DataAccess\CsvFiles\Flights.csv"""; 
        string pathToBookingCsv = """D:\\Training\2.C#\Airport Ticket Booking\Airport-Ticket-Booking\ATB\DataAccess\CsvFiles\Booking.csv""";

        return services
            .AddScoped<IFlightRepository>(provider =>
                new CsvFlightRepository(
                    provider.GetRequiredService<ICsvFileService<Flight>>(),
                    pathToFlightCsv))
            .AddScoped<IBookingRepository>(provider =>
                new CsvBookingRepository(
                    provider.GetRequiredService<ICsvFileService<Booking>>(),
                    pathToBookingCsv))
            .AddScoped<IPassengerRepository>(provider =>
                new CsvPassengerRepository(
                    provider.GetRequiredService<ICsvFileService<Passenger>>(),
                    pathToPassengerCsv))
            .AddScoped<ICsvFileService<Flight>, CsvFileService<Flight>>()
            .AddScoped<ICsvFileService<Booking>, CsvFileService<Booking>>()
            .AddScoped<ICsvFileService<Passenger>, CsvFileService<Passenger>>();
    }
}
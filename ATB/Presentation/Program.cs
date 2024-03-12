using Microsoft.Extensions.DependencyInjection;
using Presentation;

var serviceProvider = new ServiceCollection()
    .AddServices()
    .AddDataAccess()
    .AddScoped<AirportBookingSystem>();

var app = serviceProvider
    .BuildServiceProvider()
    .GetRequiredService<AirportBookingSystem>();

await app.RunMainMenu();
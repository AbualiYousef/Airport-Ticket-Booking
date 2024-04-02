using DataAccess.CsvHelperService;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories.Implementations;

public class CsvPassengerRepository(ICsvFileService<Passenger> csvFileService, string pathToCsv) : IPassengerRepository
{
    public async Task<Passenger?> GetByIdAsync(Guid id)
    {
        var passengers = await csvFileService.ReadFromCsvAsync(pathToCsv);
        return passengers.FirstOrDefault(p => p.Id == id);
    }
}
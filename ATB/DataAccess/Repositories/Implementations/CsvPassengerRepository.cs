using DataAccess.CsvHelperService;
using DataAccess.Dtos;
using DataAccess.Models;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories.Implementations;

public class CsvPassengerRepository : IPassengerRepository
{
    private readonly string _pathToCsv;
    private readonly ICsvFileService<Passenger> _csvFileService;
    
    public CsvPassengerRepository(ICsvFileService<Passenger> csvFileService,string pathToCsv)
    {
        _csvFileService = csvFileService;
        _pathToCsv = pathToCsv;
    }

    public async Task<Passenger?> GetByIdAsync(Guid id)
    {
        var passengers = await _csvFileService.ReadFromCsvAsync(_pathToCsv);
        return passengers.FirstOrDefault(p => p.Id == id);
    }
}//End of CsvPassengerRepository class

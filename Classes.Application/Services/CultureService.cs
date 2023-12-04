using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using ResultNet;

namespace Classes.Application.Services;

public interface ICultureService
{
    Task<Culture> GetByName(string cultureName);
}

public class CultureService : ICultureService
{
    private readonly ICultureRepository _cultureRepository;

    public CultureService(ICultureRepository cultureRepository)
    {
        _cultureRepository = cultureRepository;
    }
    
    public async Task<Culture> GetByName(string cultureName)
    {
        var cultureResult = await _cultureRepository.GetCultureByCodeAsync(cultureName);
        var culture = cultureResult.IsFailure() || cultureResult.Data is null ? new Culture() : cultureResult.Data;

        return culture;
    }
}
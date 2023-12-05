using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using ResultNet;

namespace Classes.Application.Services;

public interface ICultureService
{
    Task<Culture> GetByName(string cultureName);
}

public class CultureService(ICultureRepository cultureRepository) : ICultureService
{
    public async Task<Culture> GetByName(string cultureName)
    {
        var cultureResult = await cultureRepository.GetCultureByCodeAsync(cultureName);
        var culture = cultureResult.IsFailure() || cultureResult.Data is null ? new Culture() : cultureResult.Data;

        return culture;
    }
}
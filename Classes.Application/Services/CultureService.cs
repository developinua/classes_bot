using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Application.Services;

public interface ICultureService
{
    Task<Culture> GetByName(string? name);
    Task<Culture> GetByCode(string? code);
    Task<Result<string>> GetCodeByUsername(string? username);
    Task<string> GetCodeByCultureName(string? cultureName);
}

public class CultureService(
        ICultureRepository cultureRepository,
        ILogger<ICultureService> logger)
    : ICultureService
{
    public async Task<Culture> GetByName(string? name)
    {
        Culture defaultCulture = new();
        
        if (string.IsNullOrWhiteSpace(name))
            return defaultCulture;
        
        //todo: map bot culture name to db culture name
        var cultureResult = await cultureRepository.GetCultureByName(name);
        var culture = cultureResult.IsFailure() || cultureResult.Data is null
            ? defaultCulture
            : cultureResult.Data;

        return culture;
    }
    
    public async Task<Culture> GetByCode(string? code)
    {
        Culture defaultCulture = new();
        
        if (string.IsNullOrWhiteSpace(code))
            return defaultCulture;
        
        var cultureResult = await cultureRepository.GetCultureByCode(code);
        var culture = cultureResult.IsFailure() || cultureResult.Data is null
            ? defaultCulture
            : cultureResult.Data;

        return culture;
    }
    
    public async Task<Result<string>> GetCodeByUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            logger.LogWarning("Username is null or empty when try to get language code.");
            return Result.Failure<string>();
        }
        
        var cultureCode = await cultureRepository.GetCodeByUsername(username);

        if (cultureCode.IsFailure() || string.IsNullOrWhiteSpace(cultureCode.Data))
        {
            logger.LogWarning("Can't get culture code for username {Username}.", username);
            return Result.Failure<string>();
        }

        return cultureCode.Data;
    }

    public async Task<string> GetCodeByCultureName(string? cultureName)
    {
        var defaultCultureCode = new Culture().Code;

        if (string.IsNullOrWhiteSpace(cultureName))
        {
            logger.LogWarning("User culture settings is not filled in.");
            return defaultCultureCode;
        }
        
        var cultureCode = await cultureRepository.GetCodeByCultureName(cultureName);

        if (cultureCode.IsFailure() || string.IsNullOrWhiteSpace(cultureCode.Data))
        {
            logger.LogWarning("Can't get culture code for culture name {CultureName}.", cultureName);
            return defaultCultureCode;
        }

        return cultureCode.Data;
    }
}
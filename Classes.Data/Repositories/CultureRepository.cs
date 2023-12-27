using System;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Data.Repositories;

public interface ICultureRepository
{
    Task<Result<Culture?>> GetCultureByCode(string code);
    Task<Result<Culture?>> GetCultureByName(string name);
    Task<Result<string?>> GetCodeByUsername(string username);
    Task<Result<string?>> GetCodeByCultureName(string cultureName);
}

public class CultureRepository(
        PostgresDbContext dbContext,
        ILogger<CultureRepository> logger)
    : ICultureRepository
{
    public async Task<Result<Culture?>> GetCultureByCode(string code)
    {
        try
        {
            var culture = await dbContext.Cultures
                .AsNoTracking()
                .FirstOrDefaultAsync(doc => doc.Code == code);
            return Result.Success(culture);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<Culture?>().WithMessage("Can't get culture by code.");
        }
    }
    
    public async Task<Result<Culture?>> GetCultureByName(string name)
    {
        try
        {
            var culture = await dbContext.Cultures
                .AsNoTracking()
                .FirstOrDefaultAsync(doc => doc.Name == name);
            return Result.Success(culture);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<Culture?>().WithMessage("Can't get culture by name.");
        }
    }

    public async Task<Result<string?>> GetCodeByUsername(string username)
    {
        try
        {
            var cultureCode = await dbContext.Users
                .AsNoTracking()
                .Include(x => x.UserProfile.Culture)
                .Where(x => x.NickName == username)
                .Select(x => x.UserProfile.Culture.Code)
                .FirstOrDefaultAsync();
            return Result.Success(cultureCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<string?>().WithMessage("Can't get culture code by username.");
        }
    }

    public async Task<Result<string?>> GetCodeByCultureName(string cultureName)
    {
        try
        {
            var cultureCode = await dbContext.Cultures
                .AsNoTracking()
                .Where(x => x.Name == cultureName)
                .Select(x => x.Code)
                .FirstOrDefaultAsync();
            return Result.Success(cultureCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<string?>().WithMessage("Can't get culture code by culture name.");
        }
    }
}
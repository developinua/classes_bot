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
    Task<Result<Culture?>> GetCultureByCodeAsync(string languageCode);
}

public class CultureRepository(
        PostgresDbContext dbContext,
        ILogger<CultureRepository> logger)
    : ICultureRepository
{
    public async Task<Result<Culture?>> GetCultureByCodeAsync(string languageCode)
    {
        try
        {
            var culture = await dbContext.Cultures
                .AsNoTracking()
                .Where(doc => doc.LanguageCode == languageCode)
                .FirstOrDefaultAsync();
            return Result.Success(culture);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<Culture?>().WithMessage("Can't get culture by code.");
        }
    }
}
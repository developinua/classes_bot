using System;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Data.Repositories;

public interface ICultureRepository
{
    Task<Result<Culture?>> GetCultureByCodeAsync(string languageCode);
}

public class CultureRepository : ICultureRepository
{
    private readonly PostgresDbContext _dbContext;
    private readonly ILogger<CultureRepository> _logger;

    public CultureRepository(PostgresDbContext dbContext, ILogger<CultureRepository> logger) =>
        (_dbContext, _logger) = (dbContext, logger);

    public async Task<Result<Culture?>> GetCultureByCodeAsync(string languageCode)
    {
        try
        {
            var culture = await _dbContext.Cultures
                .AsNoTracking()
                .Where(doc => doc.LanguageCode == languageCode)
                .FirstOrDefaultAsync();
            return Result.Success(culture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<Culture?>().WithMessage("Can't get culture by code.");
        }
    }
}
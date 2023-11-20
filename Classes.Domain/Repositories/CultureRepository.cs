using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Classes.Domain.Repositories;

public interface ICultureRepository
{
    Task<Culture?> GetCultureByCodeAsync(string languageCode);
}

public class CultureRepository : ICultureRepository
{
    private readonly PostgresDbContext _context;
    public CultureRepository(PostgresDbContext context) => _context = context;

    public async Task<Culture?> GetCultureByCodeAsync(string languageCode) =>
        await _context.Cultures
            .Where(doc => doc.LanguageCode == languageCode)
            .FirstOrDefaultAsync();
}
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Repositories.Base;
using MongoDB.Driver;

namespace Classes.Domain.Repositories;

public interface ICultureRepository : IGenericReadonlyRepository<Culture>
{
    Task<Culture?> GetCultureByCodeAsync(string languageCode);
}

public class CultureMongoDbRepository : MongoDbRepository<Culture>, ICultureRepository
{
    public CultureMongoDbRepository(IMongoDbContext context) : base(context) {}

    public async Task<Culture?> GetCultureByCodeAsync(string languageCode)
    {
        var filter = Builders<Culture>.Filter.Eq(doc => doc.LanguageCode, languageCode);
        var filteredDocument = await DbCollection.FindAsync(filter);

        return await filteredDocument.SingleOrDefaultAsync();
    }
}
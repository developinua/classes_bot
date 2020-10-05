namespace TarasZoukClasses.Domain.Service.CultureService
{
    using System.Threading.Tasks;
    using Data.Context;
    using Data.Models;
    using Data.Repositories;
    using MongoDB.Driver;

    public class CultureMongoDbRepository : MongoDbRepository<Culture>, ICultureRepository
    {
        public CultureMongoDbRepository(IMongoDbContext context) : base(context) {}

        public async Task<Culture> GetCultureByCodeAsync(string languageCode)
        {
            var filter = Builders<Culture>.Filter.Eq(doc => doc.LanguageCode, languageCode);

            var filteredDocument = await DbCollection.FindAsync(filter);

            return await filteredDocument.SingleOrDefaultAsync();
        }
    }
}

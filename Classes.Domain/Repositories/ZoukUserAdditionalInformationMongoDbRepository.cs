using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Repositories.Base;

namespace Classes.Domain.Repositories;

public interface IZoukUserAdditionalInformationRepository : IGenericRepository<UserInformation> {}

public class ZoukUserAdditionalInformationMongoDbRepository : MongoDbRepository<UserInformation>, IZoukUserAdditionalInformationRepository
{
    public ZoukUserAdditionalInformationMongoDbRepository(IMongoDbContext context) : base(context) { }
}
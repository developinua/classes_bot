using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Repositories.Base;

namespace Classes.Domain.Repositories;

public interface IZoukUserRepository : IGenericRepository<User> {}

public class ZoukUserMongoDbRepository : MongoDbRepository<User>, IZoukUserRepository
{
    public ZoukUserMongoDbRepository(IMongoDbContext context) : base(context) {}
}
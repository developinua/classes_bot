using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Repositories.Base;

namespace Classes.Domain.Repositories;

public interface IZoukUserSubscriptionRepository : IGenericRepository<ZoukUserSubscription> { }

public class ZoukUserSubscriptionMongoDbRepository : MongoDbRepository<ZoukUserSubscription>, IZoukUserSubscriptionRepository
{
    public ZoukUserSubscriptionMongoDbRepository(IMongoDbContext context) : base(context) { }
}
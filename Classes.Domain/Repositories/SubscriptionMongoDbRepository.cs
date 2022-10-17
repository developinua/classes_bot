using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Repositories.Base;

namespace Classes.Domain.Repositories;

public interface ISubscriptionRepository : IGenericRepository<Subscription> {}

public class SubscriptionMongoDbRepository : MongoDbRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionMongoDbRepository(IMongoDbContext context) : base(context) { }
}
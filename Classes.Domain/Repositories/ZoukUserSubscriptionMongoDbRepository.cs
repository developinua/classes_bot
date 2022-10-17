using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Repositories.Base;

namespace Classes.Domain.Repositories;

public interface IUserSubscriptionRepository : IGenericRepository<UserSubscription> { }

public class UserSubscriptionMongoDbRepository : MongoDbRepository<UserSubscription>, IUserSubscriptionRepository
{
    public UserSubscriptionMongoDbRepository(IMongoDbContext context) : base(context) { }
}
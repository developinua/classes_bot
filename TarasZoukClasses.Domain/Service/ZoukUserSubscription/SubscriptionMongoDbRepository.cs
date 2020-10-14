namespace TarasZoukClasses.Domain.Service.ZoukUserSubscription
{
    using Data.Context;
    using Data.Models;
    using Data.Repositories;

    public class ZoukUserSubscriptionMongoDbRepository : MongoDbRepository<ZoukUserSubscription>, IZoukUserSubscriptionRepository
    {
        public ZoukUserSubscriptionMongoDbRepository(IMongoDbContext context) : base(context) { }
    }
}

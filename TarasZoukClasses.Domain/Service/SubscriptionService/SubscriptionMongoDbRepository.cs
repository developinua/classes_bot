namespace TarasZoukClasses.Domain.Service.SubscriptionService
{
    using Data.Context;
    using Data.Models;
    using Data.Repositories;

    public class SubscriptionMongoDbRepository : MongoDbRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionMongoDbRepository(IMongoDbContext context) : base(context) { }
    }
}

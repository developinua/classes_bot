namespace TarasZoukClasses.Domain.Service.ZoukUserPaymentService
{
    using Data.Context;
    using Data.Models;
    using Data.Repositories;

    public class ZoukUserPaymentMongoDbRepository : MongoDbRepository<ZoukUserPayment>, IZoukUserPaymentRepository
    {
        public ZoukUserPaymentMongoDbRepository(IMongoDbContext context) : base(context) {}
    }
}

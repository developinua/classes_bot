namespace TarasZoukClasses.Domain.Service.ZoukUserService
{
    using Data.Context;
    using Data.Models;
    using Data.Repositories;

    public class ZoukUserMongoDbRepository : MongoDbRepository<ZoukUser>, IZoukUserRepository
    {
        public ZoukUserMongoDbRepository(IMongoDbContext context) : base(context) {}
    }
}

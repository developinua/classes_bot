namespace TarasZoukClasses.Domain.Service.UserService
{
    using Data.Context;
    using Data.Repositories;
    using TarasZoukClasses.Data.Models;

    public class ZoukUserMongoDbRepository : MongoDbRepository<ZoukUser>, IZoukUserRepository
    {
        public ZoukUserMongoDbRepository(IMongoDbContext context) : base(context) {}
    }
}

namespace TarasZoukClasses.Domain.Service.UserService
{
    using Data.Context;
    using Data.Repositories;
    using TarasZoukClasses.Data.Models;

    public class UserMongoDbRepository : MongoDbRepository<User>, IUserRepository
    {
        public UserMongoDbRepository(IMongoDbContext context) : base(context) {}
    }
}

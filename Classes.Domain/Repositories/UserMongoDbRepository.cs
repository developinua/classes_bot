using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Repositories.Base;

namespace Classes.Domain.Repositories;

public interface IUserRepository : IGenericRepository<User> {}

public class UserMongoDbRepository : MongoDbRepository<User>, IUserRepository
{
    public UserMongoDbRepository(IMongoDbContext context) : base(context) {}
}
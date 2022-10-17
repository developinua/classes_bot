using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Repositories.Base;

namespace Classes.Domain.Repositories;

public interface IUserInformationRepository : IGenericRepository<UserInformation> {}

public class UserInformationMongoDbRepository : MongoDbRepository<UserInformation>, IUserInformationRepository
{
    public UserInformationMongoDbRepository(IMongoDbContext context) : base(context) { }
}
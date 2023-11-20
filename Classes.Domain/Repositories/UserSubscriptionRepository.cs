using Classes.Data.Context;

namespace Classes.Domain.Repositories;

public interface IUserSubscriptionRepository { }

public class UserSubscriptionRepository : IUserSubscriptionRepository
{
    private readonly PostgresDbContext _dbContext;
    
    public UserSubscriptionRepository(PostgresDbContext dbContext) => _dbContext = dbContext;
}
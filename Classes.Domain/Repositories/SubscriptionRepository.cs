using Classes.Data.Context;

namespace Classes.Domain.Repositories;

public interface ISubscriptionEditableRepository
{
}

public class SubscriptionRepository : ISubscriptionEditableRepository
{
    private readonly PostgresDbContext _dbContext;

    public SubscriptionRepository(PostgresDbContext dbContext) => _dbContext = dbContext;
}
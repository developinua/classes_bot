using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Classes.Data.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveSubscriptionByTypeAndPeriodAsync(
        SubscriptionType subscriptionGroup,
        SubscriptionPeriod subscriptionPeriod);
}

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly PostgresDbContext _dbContext;

    public SubscriptionRepository(PostgresDbContext dbContext) => _dbContext = dbContext;

    public async Task<Subscription?> GetActiveSubscriptionByTypeAndPeriodAsync(
        SubscriptionType subscriptionGroup,
        SubscriptionPeriod subscriptionPeriod)
    {
        var subscription = await _dbContext.Subscriptions
            .FirstOrDefaultAsync(x =>
                x.IsActive
                && x.Type.Equals(subscriptionGroup)
                && x.Period.Equals(subscriptionPeriod));
        return subscription;
    }
}
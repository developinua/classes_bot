using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ResultNet;

namespace Classes.Data.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveSubscriptionByTypeAndPeriodAsync(
        SubscriptionType subscriptionGroup,
        SubscriptionPeriod subscriptionPeriod);

    Task<Result> Add(List<Subscription> subscriptions);
    Task<Result> RemoveActiveSubscriptions();
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

    public async Task<Result> Add(List<Subscription> subscriptions)
    {
        await _dbContext.Subscriptions.AddRangeAsync(subscriptions);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RemoveActiveSubscriptions()
    {
        await _dbContext.Subscriptions.Where(x => x.IsActive).ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
        
        return Result.Success();
    }
}
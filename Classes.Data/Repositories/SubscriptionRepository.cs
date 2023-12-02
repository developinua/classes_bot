using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Data.Repositories;

public interface ISubscriptionRepository
{
    Task<Result<Subscription?>> GetActiveByTypeAndPeriodAsync(
        SubscriptionType subscriptionGroup,
        SubscriptionPeriod subscriptionPeriod);

    Task<Result> Add(IEnumerable<Subscription> subscriptions);
    Task<Result> RemoveAllActive();
}

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly PostgresDbContext _dbContext;
    private readonly ILogger<SubscriptionRepository> _logger;

    public SubscriptionRepository(PostgresDbContext dbContext, ILogger<SubscriptionRepository> logger) =>
        (_dbContext, _logger) = (dbContext, logger);

    public async Task<Result<Subscription?>> GetActiveByTypeAndPeriodAsync(
        SubscriptionType subscriptionGroup,
        SubscriptionPeriod subscriptionPeriod)
    {
        try
        {
            var subscription = await _dbContext.Subscriptions
                .FirstOrDefaultAsync(x =>
                    x.IsActive
                    && x.Type.Equals(subscriptionGroup)
                    && x.Period.Equals(subscriptionPeriod));
            return Result.Success(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<Subscription?>().WithMessage("Can't get active subscription by type and period.");
        }
    }

    public async Task<Result> Add(IEnumerable<Subscription> subscriptions)
    {
        try
        {
            await _dbContext.Subscriptions.AddRangeAsync(subscriptions);
            await _dbContext.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<Subscription?>().WithMessage("Can't add subscriptions.");
        }
    }

    public async Task<Result> RemoveAllActive()
    {
        try
        {
            await _dbContext.Subscriptions.Where(x => x.IsActive).ExecuteDeleteAsync();
            await _dbContext.SaveChangesAsync();
        
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<Subscription?>().WithMessage("Can't remove all active subscriptions.");
        }
    }
}
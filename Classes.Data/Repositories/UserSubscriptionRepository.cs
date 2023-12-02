using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ResultNet;

namespace Classes.Data.Repositories;

public interface IUserSubscriptionRepository
{
    Task Update(UserSubscription userSubscription);
    Task<Result<UserSubscription?>> GetById(long id);
    Task<Result<List<UserSubscription>>> GetByUsername(string username);
    Task<Result<UserSubscription?>> GetByUsernameAndType(string username, SubscriptionType subscriptionType);
    Task<Result<List<UserSubscription>>> GetActiveUserSubscriptionsWithRemainingClasses(string username);
    Task Add(UserSubscription userSubscription);
}

public class UserSubscriptionRepository : IUserSubscriptionRepository
{
    private readonly PostgresDbContext _dbContext;
    
    public UserSubscriptionRepository(PostgresDbContext dbContext) => _dbContext = dbContext;
    
    public async Task Update(UserSubscription userSubscription)
    {
        _dbContext.UsersSubscriptions.Update(userSubscription);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Result<UserSubscription?>> GetById(long id)
    {
        var userSubscription = await _dbContext.UsersSubscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(id));
        return Result.Success(userSubscription);
    }

    public async Task<Result<List<UserSubscription>>> GetByUsername(string username)
    {
        var userSubscriptions = await _dbContext.UsersSubscriptions
            .Where(x => x.User.NickName.Equals(username) && x.RemainingClasses > 0)
            .ToListAsync();
        return Result.Success(userSubscriptions);
    }

    public async Task<Result<UserSubscription?>> GetByUsernameAndType(
        string username,
        SubscriptionType subscriptionType)
    {
        var response = await _dbContext.UsersSubscriptions.FirstOrDefaultAsync(x =>
            x.User.NickName == username
            && x.Subscription.Type == subscriptionType);
        return Result.Success(response);
    }

    public async Task<Result<List<UserSubscription>>> GetActiveUserSubscriptionsWithRemainingClasses(string username)
    {
        return await _dbContext.UsersSubscriptions
            .Where(x =>
                x.User.NickName == username
                && x.RemainingClasses > 0
                && x.Subscription.IsActive)
            .ToListAsync();
    }

    public async Task Add(UserSubscription userSubscription)
    {
        await _dbContext.UsersSubscriptions.AddAsync(userSubscription);
        await _dbContext.SaveChangesAsync();
    }
}
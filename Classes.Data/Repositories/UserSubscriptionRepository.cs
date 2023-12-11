using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Extensions;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Data.Repositories;

public interface IUserSubscriptionRepository
{
    Task<Result> Add(UserSubscription userSubscription);
    Task<Result<UserSubscription?>> GetById(long id);
    Task<Result<IReadOnlyCollection<UserSubscription>>> GetByUsername(string username);
    Task<Result<UserSubscription?>> GetByUsernameAndType(string username, SubscriptionType subscriptionType);
    Task<Result<IReadOnlyCollection<UserSubscription>>> GetAllActiveWithRemainingClasses(string username);
    Task<Result> Update(UserSubscription userSubscription);
}

public class UserSubscriptionRepository(
        PostgresDbContext dbContext,
        ILogger<UserSubscriptionRepository> logger)
    : IUserSubscriptionRepository
{
    public async Task<Result> Add(UserSubscription userSubscription)
    {
        try
        {
            await dbContext.UsersSubscriptions.AddAsync(userSubscription);
            await dbContext.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User subscription hasn't been created.");
        }
    }

    public async Task<Result<UserSubscription?>> GetById(long id)
    {
        try
        {
            var userSubscription = await dbContext.UsersSubscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            return Result.Success(userSubscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<UserSubscription?>()
                .WithMessage("Can't get user subscription by id.");
        }
    }

    public async Task<Result<IReadOnlyCollection<UserSubscription>>> GetByUsername(string username)
    {
        try
        {
            var userSubscriptions = await dbContext.UsersSubscriptions
                .Where(x => x.User.NickName.Equals(username) && x.RemainingClasses > 0)
                .ToListAsync();
            return Result.Success(userSubscriptions.AsReadOnlyCollection());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<IReadOnlyCollection<UserSubscription>>()
                .WithMessage("Can't get user subscriptions by username.");
        }
    }

    public async Task<Result<UserSubscription?>> GetByUsernameAndType(
        string username,
        SubscriptionType subscriptionType)
    {
        try
        {
            var response = await dbContext.UsersSubscriptions
                .FirstOrDefaultAsync(x =>
                    x.User.NickName == username
                    && x.Subscription.Type == subscriptionType);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<UserSubscription?>()
                .WithMessage("Can't get user subscription by username and type.");
        }
    }

    public async Task<Result<IReadOnlyCollection<UserSubscription>>> GetAllActiveWithRemainingClasses(string username)
    {
        try
        {
            var response = await dbContext.UsersSubscriptions
                .Where(x =>
                    x.User.NickName == username
                    && x.RemainingClasses > 0
                    && x.Subscription.IsActive)
                .ToListAsync();
            return Result.Success(response.AsReadOnlyCollection());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<Result<IReadOnlyCollection<UserSubscription>>>()
                .WithMessage("Can't get all active user subscription by username with remaining classes.");
        }
    }
    
    public async Task<Result> Update(UserSubscription userSubscription)
    {
        try
        {
            dbContext.UsersSubscriptions.Update(userSubscription);
            await dbContext.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User subscription hasn't been updated.");
        }
    }
}
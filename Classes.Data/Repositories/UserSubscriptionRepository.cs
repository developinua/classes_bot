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

public interface IUserSubscriptionRepository
{
    Task<Result> Add(UserSubscription userSubscription);
    Task<Result<UserSubscription?>> GetById(long id);
    Task<Result<List<UserSubscription>>> GetByUsername(string username);
    Task<Result<UserSubscription?>> GetByUsernameAndType(string username, SubscriptionType subscriptionType);
    Task<Result<List<UserSubscription>>> GetAllActiveWithRemainingClasses(string username);
    Task<Result> Update(UserSubscription userSubscription);
}

public class UserSubscriptionRepository : IUserSubscriptionRepository
{
    private readonly PostgresDbContext _dbContext;
    private readonly ILogger<UserSubscriptionRepository> _logger;
    public UserSubscriptionRepository(PostgresDbContext dbContext, ILogger<UserSubscriptionRepository> logger) =>
        (_dbContext, _logger) = (dbContext, logger);
    
    public async Task<Result> Add(UserSubscription userSubscription)
    {
        try
        {
            await _dbContext.UsersSubscriptions.AddAsync(userSubscription);
            await _dbContext.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User subscription hasn't been created.");
        }
    }

    public async Task<Result<UserSubscription?>> GetById(long id)
    {
        try
        {
            var userSubscription = await _dbContext.UsersSubscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            return Result.Success(userSubscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<UserSubscription?>()
                .WithMessage("Can't get user subscription by id.");
        }
    }

    public async Task<Result<List<UserSubscription>>> GetByUsername(string username)
    {
        try
        {
            var userSubscriptions = await _dbContext.UsersSubscriptions
                .Where(x => x.User.NickName.Equals(username) && x.RemainingClasses > 0)
                .ToListAsync();
            return Result.Success(userSubscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<List<UserSubscription>>()
                .WithMessage("Can't get user subscriptions by username.");
        }
    }

    public async Task<Result<UserSubscription?>> GetByUsernameAndType(
        string username,
        SubscriptionType subscriptionType)
    {
        try
        {
            var response = await _dbContext.UsersSubscriptions.FirstOrDefaultAsync(x =>
                x.User.NickName == username
                && x.Subscription.Type == subscriptionType);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<UserSubscription?>()
                .WithMessage("Can't get user subscription by username and type.");
        }
    }

    public async Task<Result<List<UserSubscription>>> GetAllActiveWithRemainingClasses(string username)
    {
        try
        {
            var response = await _dbContext.UsersSubscriptions
                .Where(x =>
                    x.User.NickName == username
                    && x.RemainingClasses > 0
                    && x.Subscription.IsActive)
                .ToListAsync();
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<Result<List<UserSubscription>>>()
                .WithMessage("Can't get all active user subscription by username with remaining classes.");
        }
    }
    
    public async Task<Result> Update(UserSubscription userSubscription)
    {
        try
        {
            _dbContext.UsersSubscriptions.Update(userSubscription);
            await _dbContext.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User subscription hasn't been updated.");
        }
    }
}
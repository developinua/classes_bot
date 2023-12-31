﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Data.Repositories;

public interface ISubscriptionRepository
{
    Task<Result> Add(IEnumerable<Subscription> subscriptions);
    Task<Result> RemoveAllActive();
}

public class SubscriptionRepository(
        PostgresDbContext dbContext,
        ILogger<SubscriptionRepository> logger) 
    : ISubscriptionRepository
{
    public async Task<Result> Add(IEnumerable<Subscription> subscriptions)
    {
        try
        {
            await dbContext.Subscriptions.AddRangeAsync(subscriptions);
            await dbContext.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<Subscription?>().WithMessage("Can't add subscriptions.");
        }
    }

    public async Task<Result> RemoveAllActive()
    {
        try
        {
            await dbContext.Subscriptions.Where(x => x.IsActive).ExecuteDeleteAsync();
            await dbContext.SaveChangesAsync();
        
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<Subscription?>().WithMessage("Can't remove all active subscriptions.");
        }
    }
}
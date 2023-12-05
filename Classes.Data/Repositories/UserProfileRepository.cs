using System;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Data.Repositories;

public interface IUserProfileRepository
{
    Task<Result> CreateAsync(UserProfile userProfile);
    Task<Result<UserProfile?>> GetUserProfileByChatId(long chatId);
    Task<Result> UpdateAsync(UserProfile userProfile);
}

public class UserProfileRepository(
        PostgresDbContext dbContext,
        ILogger<UserProfileRepository> logger)
    : IUserProfileRepository
{
    public async Task<Result<UserProfile?>> GetUserProfileByChatId(long chatId)
    {
        try
        {
            return await dbContext.UsersProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ChatId == chatId);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<UserProfile?>().WithMessage("Problem with finding the user.");
        }
    }

    public async Task<Result> CreateAsync(UserProfile userProfile)
    {
        try
        {
            await dbContext.UsersProfiles.AddAsync(userProfile);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User hasn't been created.");
        }
    }

    public async Task<Result> UpdateAsync(UserProfile userProfile)
    {
        try
        {
            dbContext.UsersProfiles.Update(userProfile);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User hasn't been updated.");
        }
    }
}
using System;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
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

public class UserProfileRepository : IUserProfileRepository
{
    private readonly PostgresDbContext _dbContext;
    private readonly ILogger<UserProfileRepository> _logger;
    
    public UserProfileRepository(PostgresDbContext dbContext, ILogger<UserProfileRepository> logger) =>
        (_dbContext, _logger) = (dbContext, logger);

    public async Task<Result<UserProfile?>> GetUserProfileByChatId(long chatId)
    {
        try
        {
            return await _dbContext.UsersProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ChatId == chatId);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<UserProfile?>().WithMessage("Problem with finding the user.");
        }
    }

    public async Task<Result> CreateAsync(UserProfile userProfile)
    {
        try
        {
            await _dbContext.UsersProfiles.AddAsync(userProfile);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User hasn't been created.");
        }
    }

    public async Task<Result> UpdateAsync(UserProfile userProfile)
    {
        try
        {
            _dbContext.UsersProfiles.Update(userProfile);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User hasn't been updated.");
        }
    }
}
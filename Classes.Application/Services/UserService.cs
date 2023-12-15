using System;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Classes.Domain.Requests;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Application.Services;

public interface IUserService
{
    Task<Result> SaveUser(StartCallbackRequest request, Culture culture);
    Task<Culture> GetUserCulture(string? username);
}

public class UserService(
        IUpdateService updateService,
        IUserProfileService userProfileService,
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        PostgresDbContext dbContext,
        ILogger<UserService> logger)
    : IUserService
{
    public async Task<Result> SaveUser(StartCallbackRequest request, Culture culture)
    {
        var username = updateService.GetUsername(request.CallbackQuery);
        var userProfile = userProfileService.CreateUserProfile(request.CallbackQuery, culture);
        var userExists = await userRepository.GetByUsername(username) is not null;
        var result = userExists
            ? await userProfileService.UpdateUserProfile(userProfile)
            : await CreateUser(userProfile, username);

        return result;
    }

    public Task<Culture> GetUserCulture(string? username)
    {
        // todo: get from db
        return Task.FromResult(new Culture());
    }

    private async Task<Result> CreateUser(UserProfile userProfile, string nickname)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var profileCreateResult = await userProfileRepository.CreateAsync(userProfile);

            if (profileCreateResult.IsFailure())
            {
                await transaction.RollbackAsync();
                return Result.Failure().WithMessage("Failed to create user profile");
            }
            
            var userCreateResult = await userRepository.CreateAsync(new User
            {
                NickName = nickname,
                UserProfile = userProfile
            });

            if (userCreateResult.IsFailure())
            {
                await transaction.RollbackAsync();
                return Result.Failure().WithMessage("Failed to create user");
            }
                    
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogCritical(ex, "Failed to create user");

            return Result.Failure().WithMessage("Failed to create user");
        }
    }
}
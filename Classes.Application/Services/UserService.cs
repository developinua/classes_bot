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
}

public class UserService : IUserService
{
    private readonly IUpdateService _updateService;
    private readonly IUserProfileService _userProfileService;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly PostgresDbContext _dbContext;
    private readonly ILogger<UserService> _logger;
    
    public UserService(
        IUpdateService updateService,
        IUserProfileService userProfileService,
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        PostgresDbContext dbContext,
        ILogger<UserService> logger)
    {
        _updateService = updateService;
        _userProfileService = userProfileService;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result> SaveUser(StartCallbackRequest request, Culture culture)
    {
        var username = _updateService.GetUsername(request.CallbackQuery);
        var userProfile = _userProfileService.CreateUserProfile(request.CallbackQuery, culture);
        var userExists = await _userRepository.GetByUsername(username) is not null;
        var result = userExists
            ? await _userProfileService.UpdateUserProfile(userProfile)
            : await CreateUser(userProfile, username);

        return result;
    }
    
    private async Task<Result> CreateUser(UserProfile userProfile, string nickname)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var profileCreateResult = await _userProfileRepository.CreateAsync(userProfile);

            if (profileCreateResult.IsFailure())
            {
                await transaction.RollbackAsync();
                return Result.Failure().WithMessage("Failed to create user profile");
            }
            
            var userCreateResult = await _userRepository.CreateAsync(new User
            {
                NickName = nickname,
                UserProfile = userProfile
            });

            if (userCreateResult.IsFailure())
            {
                await transaction.RollbackAsync();
                return Result.Failure().WithMessage("Failed to create user");
            }
                    
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogCritical(ex, "Failed to create user");

            return Result.Failure().WithMessage("Failed to create user");
        }
    }
}
using System;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Data.Repositories;

public interface IUserRepository
{
    Task<Result<User?>> GetByUsername(string username);
    Task<Result> CreateAsync(User user);
}

public class UserRepository(
        PostgresDbContext dbContext,
        ILogger<UserRepository> logger)
    : IUserRepository
{
    public async Task<Result> CreateAsync(User user)
    {
        try
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User hasn't been created.");
        }
    }
    
    public async Task<Result<User?>> GetByUsername(string username)
    {
        try
        {
           return await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NickName.Equals(username));
        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Failure<User?>().WithMessage("Can't get user by username.");
        }
    }
}
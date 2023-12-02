using System;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Data.Repositories;

public interface IUserRepository
{
    Task<Result<User?>> GetByUsername(string username);
    Task<Result> CreateAsync(User user);
}

public class UserRepository : IUserRepository
{
    private readonly PostgresDbContext _dbContext;
    private readonly ILogger<UserRepository> _logger;
    
    public UserRepository(PostgresDbContext dbContext, ILogger<UserRepository> logger) =>
        (_dbContext, _logger) = (dbContext, logger);

    public async Task<Result> CreateAsync(User user)
    {
        try
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure().WithMessage("User hasn't been created.");
        }
    }
    
    public async Task<Result<User?>> GetByUsername(string username)
    {
        try
        {
           return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NickName.Equals(username));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<User?>().WithMessage("Can't get user by username.");
        }
    }
}
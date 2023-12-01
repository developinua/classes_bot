using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Microsoft.EntityFrameworkCore;
using ResultNet;

namespace Classes.Data.Repositories;

public interface IUserSubscriptionRepository
{
    Task Update(UserSubscription userSubscription);
    Task<Result<UserSubscription?>> GetById(long id);
    Task<Result<List<UserSubscription>>> GetByUsername(string username);
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
}
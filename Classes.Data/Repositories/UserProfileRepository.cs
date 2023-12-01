using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Classes.Data.Repositories;

public interface IUserProfileRepository
{
    Task CreateAsync(UserProfile userProfile);
    Task<UserProfile?> GetUserProfileByChatId(long chatId);
    Task UpdateAsync(UserProfile userProfile);
}

public class UserProfileEditableRepository : IUserProfileRepository
{
    private readonly PostgresDbContext _dbContext;
    
    public UserProfileEditableRepository(PostgresDbContext dbContext) => _dbContext = dbContext;
    public async Task<UserProfile?> GetUserProfileByChatId(long chatId)
    {
        var userProfileDb = await _dbContext.UsersProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChatId == chatId);
        return userProfileDb;
    }

    public async Task CreateAsync(UserProfile userProfile)
    {
        await _dbContext.UsersProfiles.AddAsync(userProfile);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserProfile userProfile)
    {
        _dbContext.UsersProfiles.Update(userProfile);
        await _dbContext.SaveChangesAsync();
    }
}
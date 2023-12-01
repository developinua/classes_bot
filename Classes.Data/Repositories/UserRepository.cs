using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Classes.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByNickname(string nickName);
    Task CreateAsync(User user);
}

public class UserRepository : IUserRepository
{
    private readonly PostgresDbContext _dbContext;
    
    public UserRepository(PostgresDbContext dbContext) => _dbContext = dbContext;

    public async Task CreateAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<User?> GetUserByNickname(string nickName) =>
        await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NickName.Equals(nickName));
}
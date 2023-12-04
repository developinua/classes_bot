using Classes.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Classes.Data.Context;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserProfile> UsersProfiles { get; set; }
    public DbSet<UserSubscription> UsersSubscriptions { get; set; }
    public DbSet<Culture> Cultures { get; set; }
    public DbSet<Command> Commands { get; set; }
}
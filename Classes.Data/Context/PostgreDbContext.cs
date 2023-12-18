using Classes.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Classes.Data.Context;

public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<UserProfile> UsersProfiles { get; set; } = null!;
    public DbSet<UserSubscription> UsersSubscriptions { get; set; } = null!;
    public DbSet<Culture> Cultures { get; set; } = null!;
    public DbSet<Command> Commands { get; set; } = null!;
    
    // // Configure your models (if needed)
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //
    //     // Model configuration goes here
    // }
}
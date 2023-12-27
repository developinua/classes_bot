using Classes.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Classes.Data.Context;

public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; } = null!;
    public DbSet<Subscription> Subscriptions { get; init; } = null!;
    public DbSet<UserProfile> UserProfiles { get; init; } = null!;
    public DbSet<UserSubscription> UserSubscriptions { get; init; } = null!;
    public DbSet<Culture> Cultures { get; init; } = null!;
    public DbSet<Command> Commands { get; init; } = null!;
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .ToTable("User")
            .HasQueryFilter(x => x.IsActive)
            .HasKey(k => k.Id);
        
        builder.Entity<Subscription>()
            .ToTable("Subscription")
            .HasQueryFilter(x => x.IsActive)
            .HasKey(k => k.Id);
        
        builder.Entity<UserProfile>()
            .ToTable("UserProfile")
            .HasKey(k => k.Id);
        
        builder.Entity<UserSubscription>()
            .ToTable("UserSubscription")
            .HasQueryFilter(x =>
                x.Subscription.IsActive
                && x.RemainingClasses > 0)
            .HasKey(k => k.Id);
        
        builder.Entity<Culture>()
            .ToTable("Culture")
            .HasKey(k => k.Id);
        
        builder.Entity<Command>()
            .ToTable("Command")
            .HasQueryFilter(x => x.IsActive)
            .HasKey(k => k.Id);
    }
}
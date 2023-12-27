using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Classes.Application.Services;

public static class MigrationService
{
    public static async Task MigrateDbAsync(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<BotService>>();

        try
        {
            await dbContext.Database.MigrateAsync();
            await SeedData(dbContext);
            logger.LogInformation("Successfully migrated the database.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    private static async Task SeedData(PostgresDbContext dbContext)
    {
        const string adminNickName = "nazikBro";
        const string enCode = "en-US";
        const string ukCode = "uk-UA";

        var date = DateTime.UtcNow;
        
        var cultureEn = await dbContext.Cultures.FirstOrDefaultAsync(x => x.Code == enCode);
        if (cultureEn is null)
        {
            cultureEn = new Culture
            {
                Name = "English",
                Code = enCode,
                CreatedAt = date
            };
            await dbContext.Cultures.AddAsync(cultureEn);
            await dbContext.SaveChangesAsync();
        }
        
        var cultureUk = await dbContext.Cultures.FirstOrDefaultAsync(x => x.Code == ukCode);
        if (cultureUk is null)
        {
            cultureUk = new Culture
            {
                Name = "Ukrainian",
                Code = ukCode,
                CreatedAt = date
            };
            await dbContext.Cultures.AddAsync(cultureUk);
            await dbContext.SaveChangesAsync();
        }
        
        var nazikUser = await dbContext.Users.FirstOrDefaultAsync(x => x.NickName == adminNickName);
        if (nazikUser is null)
        {
            nazikUser = new User
            {
                NickName = adminNickName,
                UserProfile = new UserProfile
                {
                    ChatId = 0,
                    FirstName = "Nazar",
                    LastName = "Bulyha",
                    IsPremium = true,
                    IsBot = false,
                    Culture = cultureEn,
                    CreatedAt = date
                },
                IsActive = true,
                CreatedAt = date
            };
            
            await dbContext.Users.AddAsync(nazikUser);
            await dbContext.SaveChangesAsync();
        }
        
        var premiumClassSubscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(x => x.ClassesCount == int.MaxValue && x.Type == SubscriptionType.Class);
        if (premiumClassSubscription is null)
        {
            premiumClassSubscription = new Subscription
            {
                Name = "Premium class",
                Description = "Premium class with access to all features.",
                ClassesCount = int.MaxValue,
                Price = 100,
                DiscountPercent = 99.99m,
                Type = SubscriptionType.Class,
                IsActive = true,
                CreatedAt = date
            };
            
            await dbContext.Subscriptions.AddAsync(premiumClassSubscription);
            await dbContext.SaveChangesAsync();
        }
        
        var premiumCourseSubscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(x => x.ClassesCount == int.MaxValue && x.Type == SubscriptionType.Course);
        if (premiumCourseSubscription is null)
        {
            premiumCourseSubscription = new Subscription
            {
                Name = "Premium course",
                Description = "Premium course with access to all features.",
                ClassesCount = int.MaxValue,
                Price = 100,
                DiscountPercent = 99.99m,
                Type = SubscriptionType.Course,
                IsActive = true,
                CreatedAt = date
            };
            
            await dbContext.Subscriptions.AddAsync(premiumCourseSubscription);
            await dbContext.SaveChangesAsync();
        }

        var adminHasSubscriptions = dbContext.UserSubscriptions.Any(x => x.User.NickName == adminNickName);
        if (!adminHasSubscriptions)
        {
            var userSubscriptions = new List<UserSubscription>
            {
                new()
                {
                    User = nazikUser,
                    Subscription = premiumClassSubscription,
                    RemainingClasses = int.MaxValue,
                    CreatedAt = date
                },
                new()
                {
                    User = nazikUser,
                    Subscription = premiumCourseSubscription,
                    RemainingClasses = int.MaxValue,
                    CreatedAt = date
                }
            };

            await dbContext.UserSubscriptions.AddRangeAsync(userSubscriptions);
            await dbContext.SaveChangesAsync();
        }
    }
}
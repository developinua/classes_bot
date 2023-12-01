using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Commands.Administration;

public class SeedCommand : IBotCommand
{
    public string Name => "/seed";

    public string CallbackQueryPattern => "Not implemented";

    public bool Contains(Message message) => message.Type == MessageType.Text && message.Text!.Contains(Name);

    public bool Contains(string callbackQueryData) =>
        new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

    public async Task Execute(Message message, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        var chatId = message.From!.Id;

        if (!CanExecuteCommand(message.From.Username!))
        {
            await client.SendTextMessageAsync(chatId, "Access denied. You can't execute this command.");
            return;
        }

        await ProcessSubscriptions(dbContext);
        await ProcessUserSubscriptions(dbContext);

        await client.SendTextMessageAsync(chatId, "*Successfully seeded*", parseMode: ParseMode.Markdown);
    }

    public Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        return Task.CompletedTask;
    }
    
    // todo: extract to separate class
    private static bool CanExecuteCommand(string username)
    {
        var allowedUsers = new[] { "nazikBro", "taras_zouk", "kovalinas" };
        return allowedUsers.Any(x => x.Equals(username));
    }

    private static async Task ProcessSubscriptions(PostgresDbContext dbContext)
    {
        await dbContext.Subscriptions.Where(x => x.IsActive).ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();
        
        var subscriptions = new List<Subscription>
        {
            #region SubscriptionType.None

            new()
            {
                Name = "Whoops",
                Description = "Nothing to do here",
                Price = 0,
                DiscountPercent = 0,
                Classes = 0,
                Type = SubscriptionType.None,
                Period = SubscriptionPeriod.None,
                IsActive = true
            },

            #endregion

            #region SubscriptionType.Novice

            new()
            {
                Name = "One class",
                Description = "One class",
                Price = 200,
                DiscountPercent = 0,
                Classes = 1,
                Type = SubscriptionType.Novice,
                Period = SubscriptionPeriod.Day,
                IsActive = true
            },
            new()
            {
                Name = "Week Classes",
                Description = "Two classes",
                Price = 400,
                DiscountPercent = 0,
                Classes = 2,
                Type = SubscriptionType.Novice,
                Period = SubscriptionPeriod.Week,
                IsActive = true
            },
            new()
            {
                Name = "Two Week Classes",
                Description = "Four classes",
                Price = 800,
                DiscountPercent = 0,
                Classes = 4,
                Type = SubscriptionType.Novice,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "One Month Classes",
                Description = "Eight classes",
                Price = 1600,
                DiscountPercent = 0,
                Classes = 8,
                Type = SubscriptionType.Novice,
                Period = SubscriptionPeriod.Month,
                IsActive = true
            },
            new()
            {
                Name = "Three Months Classes",
                Description = "Sixteen classes",
                Price = 3200,
                DiscountPercent = 15,
                Classes = 16,
                Type = SubscriptionType.Novice,
                Period = SubscriptionPeriod.ThreeMonths,
                IsActive = true
            },

            #endregion

            #region SubscriptionType.Medium

            new()
            {
                Name = "One class",
                Description = "One class",
                Price = 200,
                DiscountPercent = 0,
                Classes = 1,
                Type = SubscriptionType.Intermediate,
                Period = SubscriptionPeriod.Day,
                IsActive = true
            },
            new()
            {
                Name = "Week Classes",
                Description = "Two classes",
                Price = 400,
                DiscountPercent = 0,
                Classes = 2,
                Type = SubscriptionType.Intermediate,
                Period = SubscriptionPeriod.Week,
                IsActive = true
            },
            new()
            {
                Name = "Two Week Classes",
                Description = "Four classes",
                Price = 800,
                DiscountPercent = 0,
                Classes = 4,
                Type = SubscriptionType.Intermediate,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "One Month Classes",
                Description = "Eight classes",
                Price = 1600,
                DiscountPercent = 0,
                Classes = 8,
                Type = SubscriptionType.Intermediate,
                Period = SubscriptionPeriod.Month,
                IsActive = true
            },
            new()
            {
                Name = "Three Months Classes",
                Description = "Sixteen classes",
                Price = 3200,
                DiscountPercent = 15,
                Classes = 16,
                Type = SubscriptionType.Intermediate,
                Period = SubscriptionPeriod.ThreeMonths,
                IsActive = true
            },

            #endregion

            #region SubscriptionType.LadyStyling

            new()
            {
                Name = "One class",
                Description = "One class",
                Price = 200,
                DiscountPercent = 0,
                Classes = 1,
                Type = SubscriptionType.LadyStyling,
                Period = SubscriptionPeriod.Day,
                IsActive = true
            },
            new()
            {
                Name = "Week Classes",
                Description = "Two classes",
                Price = 400,
                DiscountPercent = 0,
                Classes = 2,
                Type = SubscriptionType.LadyStyling,
                Period = SubscriptionPeriod.Week,
                IsActive = true
            },
            new()
            {
                Name = "Two Week Classes",
                Description = "Four classes",
                Price = 800,
                DiscountPercent = 0,
                Classes = 4,
                Type = SubscriptionType.LadyStyling,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "One Month Classes",
                Description = "Eight classes",
                Price = 1600,
                DiscountPercent = 0,
                Classes = 8,
                Type = SubscriptionType.LadyStyling,
                Period = SubscriptionPeriod.Month,
                IsActive = true
            },
            new()
            {
                Name = "Three Months Classes",
                Description = "Sixteen classes",
                Price = 3200,
                DiscountPercent = 15,
                Classes = 16,
                Type = SubscriptionType.LadyStyling,
                Period = SubscriptionPeriod.ThreeMonths,
                IsActive = true
            },

            #endregion
            
            #region SubscriptionType.ManStyling

            new()
            {
                Name = "One class",
                Description = "One class",
                Price = 200,
                DiscountPercent = 0,
                Classes = 1,
                Type = SubscriptionType.ManStyling,
                Period = SubscriptionPeriod.Day,
                IsActive = true
            },
            new()
            {
                Name = "Week Classes",
                Description = "Two classes",
                Price = 400,
                DiscountPercent = 0,
                Classes = 2,
                Type = SubscriptionType.ManStyling,
                Period = SubscriptionPeriod.Week,
                IsActive = true
            },
            new()
            {
                Name = "Two Week Classes",
                Description = "Four classes",
                Price = 800,
                DiscountPercent = 0,
                Classes = 4,
                Type = SubscriptionType.ManStyling,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "One Month Classes",
                Description = "Eight classes",
                Price = 1600,
                DiscountPercent = 0,
                Classes = 8,
                Type = SubscriptionType.ManStyling,
                Period = SubscriptionPeriod.Month,
                IsActive = true
            },
            new()
            {
                Name = "Three Months Classes",
                Description = "Sixteen classes",
                Price = 3200,
                DiscountPercent = 15,
                Classes = 16,
                Type = SubscriptionType.ManStyling,
                Period = SubscriptionPeriod.ThreeMonths,
                IsActive = true
            },

            #endregion

            #region SubscriptionType.Premium

            new()
            {
                Name = "All classes",
                Description = "Novice, medium, lady style classes for week",
                Price = 1200,
                DiscountPercent = 5,
                Classes = 12,
                Type = SubscriptionType.Premium,
                Period = SubscriptionPeriod.Week,
                IsActive = true
            },
            new()
            {
                Name = "All classes",
                Description = "Novice, medium, lady style classes for two weeks",
                Price = 2400,
                DiscountPercent = 5,
                Classes = 24,
                Type = SubscriptionType.Premium,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "All classes",
                Description = "Novice, medium, lady style classes for month",
                Price = 4800,
                DiscountPercent = 10,
                Classes = 48,
                Type = SubscriptionType.Premium,
                Period = SubscriptionPeriod.Month,
                IsActive = true
            },
            new()
            {
                Name = "All classes",
                Description = "Novice, medium, lady style classes for three months",
                Price = 9600,
                DiscountPercent = 15,
                Classes = 144,
                Type = SubscriptionType.Premium,
                Period = SubscriptionPeriod.ThreeMonths,
                IsActive = true
            }

            #endregion
        };

        await dbContext.Subscriptions.AddRangeAsync(subscriptions);
    }

    private static async Task ProcessUserSubscriptions(PostgresDbContext dbContext)
    {
        var userNazar = await dbContext.Users.FirstOrDefaultAsync(x => x.NickName.Equals("nazikBro"));
        var subscriptionPremiumMonth = await dbContext.Subscriptions
            .FirstOrDefaultAsync(x =>
                x.Type == SubscriptionType.Premium
                && x.Period == SubscriptionPeriod.Month);

        if (userNazar is null || subscriptionPremiumMonth is null)
            throw new Exception("Invalid admin subscriptions data in db.");
        
        var userSubscriptionPremiumMonth = new UserSubscription
        {
            User = userNazar,
            Subscription = subscriptionPremiumMonth,
            RemainingClasses = subscriptionPremiumMonth.Classes
        };
        var premiumSubscriptionInDb = dbContext.UsersSubscriptions.Where(x =>
            x.User.NickName == userSubscriptionPremiumMonth.User.NickName
            && x.Subscription.Type == userSubscriptionPremiumMonth.Subscription.Type);

        if (!premiumSubscriptionInDb.Any())
            await dbContext.UsersSubscriptions.AddAsync(userSubscriptionPremiumMonth);
    }
}
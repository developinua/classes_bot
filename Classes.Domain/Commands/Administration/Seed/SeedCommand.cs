using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Classes.Domain.Commands.Administration.Admin;
using Classes.Domain.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Commands.Administration.Seed;

public class SeedCommand : IBotCommand
{
    public string Name => "/seed";

    public string CallbackQueryPattern => "Not implemented";

    public bool Contains(Message message) => message.Type == MessageType.Text && message.Text!.Contains(Name);

    public bool Contains(string callbackQueryData) =>
        new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

    public async Task Execute(Message message, ITelegramBotClient client, IUnitOfWork services)
    {
        var chatId = message.From!.Id;

        if (!AdministrationHelper.CanExecuteCommand(message.From.Username!))
        {
            await client.SendTextMessageAsync(chatId, "Access denied. You can't execute this command.");
            return;
        }

        await ProcessSubscriptions(services);
        await ProcessUserSubscriptions(services);

        await client.SendTextMessageAsync(chatId, "*Successfully seeded*", parseMode: ParseMode.Markdown);
    }

    public Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, IUnitOfWork services)
    {
        return Task.CompletedTask;
    }

    private static async Task ProcessSubscriptions(IUnitOfWork services)
    {
        await services.Subscriptions.DeleteManyAsync(x => x.IsActive);

        var subscriptions = new List<Subscription>
        {
            #region SubscriptionType.None

            new()
            {
                Name = "Whoops",
                Description = "Nothing to do here",
                Price = 0,
                DiscountPercent = 0,
                ClassesCount = 0,
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
                ClassesCount = 1,
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
                ClassesCount = 2,
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
                ClassesCount = 4,
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
                ClassesCount = 8,
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
                ClassesCount = 16,
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
                ClassesCount = 1,
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
                ClassesCount = 2,
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
                ClassesCount = 4,
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
                ClassesCount = 8,
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
                ClassesCount = 16,
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
                ClassesCount = 1,
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
                ClassesCount = 2,
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
                ClassesCount = 4,
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
                ClassesCount = 8,
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
                ClassesCount = 16,
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
                ClassesCount = 1,
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
                ClassesCount = 2,
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
                ClassesCount = 4,
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
                ClassesCount = 8,
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
                ClassesCount = 16,
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
                ClassesCount = 12,
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
                ClassesCount = 24,
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
                ClassesCount = 48,
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
                ClassesCount = 144,
                Type = SubscriptionType.Premium,
                Period = SubscriptionPeriod.ThreeMonths,
                IsActive = true
            }

            #endregion
        };

        await services.Subscriptions.InsertManyAsync(subscriptions);
    }

    private static async Task ProcessUserSubscriptions(IUnitOfWork services)
    {
        var userNazar = await services.Users.FindOneAsync(x => x.NickName.Equals("nazikBro"));
        var subscriptionPremiumMonth = await services.Subscriptions.FindOneAsync(x =>
            x.Type == SubscriptionType.Premium
            && x.Period == SubscriptionPeriod.Month);

        if (userNazar is null || subscriptionPremiumMonth is null)
            throw new Exception("Invalid admin subscriptions data in db.");
        
        var userSubscriptionPremiumMonth = new UserSubscription
        {
            User = userNazar,
            Subscription = subscriptionPremiumMonth,
            RemainingClassesCount = subscriptionPremiumMonth.ClassesCount
        };
        var premiumSubscriptionInDb = await services.UsersSubscriptions.FilterBy(x =>
            x.User.NickName == userSubscriptionPremiumMonth.User.NickName
            && x.Subscription.Type == userSubscriptionPremiumMonth.Subscription.Type);

        if (!premiumSubscriptionInDb.Any())
            await services.UsersSubscriptions.InsertAsync(userSubscriptionPremiumMonth);
    }
}
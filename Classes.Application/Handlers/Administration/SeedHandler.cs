using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Data.Extensions;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using Classes.Domain.Requests;
using MediatR;
using ResultNet;

namespace Classes.Application.Handlers.Administration;

public class SeedHandler(
        IBotService botService,
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository,
        IUserSubscriptionRepository userSubscriptionRepository)
    : IRequestHandler<SeedRequest, Result>
{
    public async Task<Result> Handle(SeedRequest request, CancellationToken cancellationToken)
    {
        botService.UseChat(request.ChatId);
        
        if (!CanExecuteCommand(request.Username))
        {
            const string errorMessage = "Access denied. You can't execute this command.";
            await botService.SendTextMessageAsync(errorMessage, cancellationToken);
            return Result.Failure().WithMessage(errorMessage);
        }

        var processSubscriptions = await AddDefaultSubscriptions();
        var processUserSubscriptions = await AddAdminSubscription();
        
        if (processSubscriptions.IsFailure() || processUserSubscriptions.IsFailure())
            return Result.Failure().WithMessage("Error while seeding data.");

        await botService.SendTextMessageAsync("*Successfully seeded*", cancellationToken);
        
        return Result.Success();
    }
    
    // todo: extract to separate class
    private static bool CanExecuteCommand(string username)
    {
        var allowedUsers = new[] { "nazikBro", "taras_zouk", "kovalinas" };
        return allowedUsers.Any(x => x.Equals(username));
    }

    private async Task<Result> AddDefaultSubscriptions()
    {
        await subscriptionRepository.RemoveAllActive();
        
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "Two Week Classes",
                Description = "Four classes",
                Price = 800,
                DiscountPercent = 0,
                ClassesCount = 4,
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.ThreeMonth,
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "Two Week Classes",
                Description = "Four classes",
                Price = 800,
                DiscountPercent = 0,
                ClassesCount = 4,
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.ThreeMonth,
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "Two Week Classes",
                Description = "Four classes",
                Price = 800,
                DiscountPercent = 0,
                ClassesCount = 4,
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.ThreeMonth,
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "Two Week Classes",
                Description = "Four classes",
                Price = 800,
                DiscountPercent = 0,
                ClassesCount = 4,
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.ThreeMonth,
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
                Type = SubscriptionType.Class,
                Period = SubscriptionPeriod.HalfMonth,
                IsActive = true
            },
            new()
            {
                Name = "All classes",
                Description = "Novice, medium, lady style classes for two weeks",
                Price = 2400,
                DiscountPercent = 5,
                ClassesCount = 24,
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Class,
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
                Type = SubscriptionType.Course,
                Period = SubscriptionPeriod.ThreeMonth,
                IsActive = true
            }

            #endregion
        };

        await subscriptionRepository.Add(subscriptions);
        
        var userNazar = await userRepository.GetByUsername("nazikBro");
        
        if (userNazar.Data is null)
            return Result.Failure().WithMessage("Invalid admin subscriptions data in db.");
        
        var userSubscriptions = subscriptions.Select(x => new UserSubscription
        {
            User = userNazar.Data,
            Subscription = x,
            RemainingClasses = x.ClassesCount
        }).AsReadOnlyCollection();
        
        await userSubscriptionRepository.AddRange(userSubscriptions);
        
        return Result.Success();
    }

    private async Task<Result> AddAdminSubscription()
    {
        var userNazar = await userRepository.GetByUsername("nazikBro");
        var subscriptionPremiumMonth = await subscriptionRepository.GetActiveByTypeAndPeriod(
            SubscriptionType.Class, SubscriptionPeriod.Month);

        if (userNazar.Data is null || subscriptionPremiumMonth.Data is null)
            return Result.Failure().WithMessage("Invalid admin subscriptions data in db.");
        
        var userSubscriptionPremiumMonth = new UserSubscription
        {
            User = userNazar.Data!,
            Subscription = subscriptionPremiumMonth.Data,
            RemainingClasses = subscriptionPremiumMonth.Data.ClassesCount
        };
        var premiumSubscriptionInDb = await userSubscriptionRepository.GetByUsernameAndType(
                userSubscriptionPremiumMonth.User.NickName,
                userSubscriptionPremiumMonth.Subscription.Type);

        if (premiumSubscriptionInDb.Data is null)
            await userSubscriptionRepository.Add(userSubscriptionPremiumMonth);
        
        return Result.Success();
    }
}
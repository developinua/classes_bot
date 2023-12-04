using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using Classes.Domain.Requests;
using MediatR;
using ResultNet;

namespace Classes.Application.Handlers.Administration;

public class SeedHandler : IRequestHandler<SeedRequest, Result>
{
    private readonly IBotService _botService;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;

    public SeedHandler(
        IBotService botService,
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository,
        IUserSubscriptionRepository userSubscriptionRepository)
    {
        _botService = botService;
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
        _userSubscriptionRepository = userSubscriptionRepository;
    }

    public async Task<Result> Handle(SeedRequest request, CancellationToken cancellationToken)
    {
        if (!CanExecuteCommand(request.Username))
        {
            await _botService.SendTextMessageAsync(
                request.ChatId,
                "Access denied. You can't execute this command.",
                cancellationToken);
            return Result.Failure()
                .WithMessage("Access denied. You can't execute this command.");
        }

        await ProcessSubscriptions();
        await ProcessUserSubscriptions();

        await _botService.SendTextMessageAsync(
            request.ChatId,
            "*Successfully seeded*", 
            cancellationToken);
        
        return Result.Success();
    }
    
    // todo: extract to separate class
    private static bool CanExecuteCommand(string username)
    {
        var allowedUsers = new[] { "nazikBro", "taras_zouk", "kovalinas" };
        return allowedUsers.Any(x => x.Equals(username));
    }

    private async Task ProcessSubscriptions()
    {
        await _subscriptionRepository.RemoveAllActive();
        
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

        await _subscriptionRepository.Add(subscriptions);
    }

    private async Task ProcessUserSubscriptions()
    {
        var userNazar = await _userRepository.GetByUsername("nazikBro");
        var subscriptionPremiumMonth = await _subscriptionRepository.GetActiveByTypeAndPeriodAsync(
            SubscriptionType.Premium, SubscriptionPeriod.Month);

        if (userNazar is null || subscriptionPremiumMonth.Data is null)
            throw new Exception("Invalid admin subscriptions data in db.");
        
        var userSubscriptionPremiumMonth = new UserSubscription
        {
            User = userNazar.Data!,
            Subscription = subscriptionPremiumMonth.Data,
            RemainingClasses = subscriptionPremiumMonth.Data.Classes
        };
        var premiumSubscriptionInDb = await _userSubscriptionRepository.GetByUsernameAndType(
                userSubscriptionPremiumMonth.User.NickName,
                userSubscriptionPremiumMonth.Subscription.Type);

        if (premiumSubscriptionInDb.Data is not null)
            await _userSubscriptionRepository.Add(userSubscriptionPremiumMonth);
    }
}
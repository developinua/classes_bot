using System.Collections.Generic;
using System.Threading.Tasks;
using Classes.Application.Handlers.Subscriptions;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using Microsoft.Extensions.Localization;
using ResultNet;

namespace Classes.Application.Services;

public interface IUserSubscriptionService
{
    Task<Result<UserSubscription?>> GetById(long id);
    string GetUserSubscriptionInformation(UserSubscription userSubscription);
    Task<Result<IReadOnlyCollection<UserSubscription>>> GetUserSubscriptions(string username);
    Task<Result<IReadOnlyCollection<UserSubscription>>> GetUserSubscriptionsByType(
        string username, SubscriptionType subscriptionType);
    Result<bool> CanCheckinOnClass(UserSubscription userSubscription);
    Task<Result> CheckinOnClass(UserSubscription userSubscription);
}

public class UserSubscriptionService(
        IUserSubscriptionRepository userSubscriptionRepository,
        IStringLocalizer<SubscriptionsHandler> localizer)
    : IUserSubscriptionService
{
    public async Task<Result<UserSubscription?>> GetById(long id) =>
        await userSubscriptionRepository.GetById(id);

    public string GetUserSubscriptionInformation(UserSubscription userSubscription)
    {
        // todo: add to localization
        var subscription = userSubscription.Subscription;
        var priceText = subscription.DiscountPercent == 0
            ? localizer["SubscriptionPrice", subscription.Price]
            : localizer["SubscriptionPriceWithDiscount", subscription.GetPriceWithDiscount()];
        var subscriptionInformation =
            localizer["SubscriptionName", subscription.Name] +
            priceText +
            localizer["SubscriptionDescription", subscription.Description ?? string.Empty] +
            localizer["SubscriptionType", subscription.Type] +
            localizer["SubscriptionTotalClasses", subscription.ClassesCount] +
            localizer["SubscriptionRemainingClasses", userSubscription.RemainingClasses] +
            localizer["BackToSubscriptions"];

        return subscriptionInformation;
    }

    public async Task<Result<IReadOnlyCollection<UserSubscription>>> GetUserSubscriptions(string username) =>
        await userSubscriptionRepository.GetUserSubscriptions(username);

    public async Task<Result<IReadOnlyCollection<UserSubscription>>> GetUserSubscriptionsByType(
        string username, SubscriptionType subscriptionType) =>
        await userSubscriptionRepository.GetUserSubscriptionsByType(username, subscriptionType);

    public Result<bool> CanCheckinOnClass(UserSubscription userSubscription) =>
        userSubscription.RemainingClasses == 0
            ? Result.Failure<bool>().WithMessage("No available classes.")
            : Result.Success(true);

    public async Task<Result> CheckinOnClass(UserSubscription userSubscription)
    {
        if (userSubscription.RemainingClasses == 0)
            return Result.Failure().WithMessage("No available classes.");
        
        userSubscription.RemainingClasses--;
        return await userSubscriptionRepository.Update(userSubscription);
    }
}
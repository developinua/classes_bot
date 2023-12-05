using System.Threading;
using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Microsoft.Extensions.Logging;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Services;

public interface IUserSubscriptionService
{
    Task<Result<UserSubscription?>> GetFromCallback(CallbackQuery callback, string callbackPattern);
    Task<Result> CheckinOnClass(UserSubscription userSubscription);
}

public class UserSubscriptionService(
        ICallbackExtractorService callbackExtractorService,
        IUserSubscriptionRepository userSubscriptionRepository,
        ILogger<UserService> logger)
    : IUserSubscriptionService
{
    public async Task<Result<UserSubscription?>> GetFromCallback(CallbackQuery callback, string callbackPattern)
    {
        var userSubscriptionId = callbackExtractorService.GetUserSubscriptionIdFromCallback(
            callback.Data!, callbackPattern);
        return await userSubscriptionRepository.GetById(userSubscriptionId);
    }

    public async Task<Result> CheckinOnClass(UserSubscription userSubscription)
    {
        if (userSubscription.RemainingClasses == 0)
            return Result.Failure()
                .WithMessage("You haven't any available classes. Press /subscriptions to manage your subscriptions.");
        
        userSubscription.RemainingClasses--;
        return await userSubscriptionRepository.Update(userSubscription);
    }
}
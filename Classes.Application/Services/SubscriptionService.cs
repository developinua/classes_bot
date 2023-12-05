using System.Threading;
using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Services;

public interface ISubscriptionService
{
    Task<Result<Subscription>> GetSubscriptionFromCallback(
        CallbackQuery callback,
        long chatId,
        CancellationToken cancellationToken);
}

public class SubscriptionService(
        IBotService botService,
        ICallbackExtractorService callbackExtractorService,
        ISubscriptionRepository subscriptionRepository)
    : ISubscriptionService
{
    public async Task<Result<Subscription>> GetSubscriptionFromCallback(
        CallbackQuery callback,
        long chatId,
        CancellationToken cancellationToken)
    {
        var subscriptionType = callbackExtractorService.GetSubscriptionType(callback.Data!);
        var subscriptionPeriod = callbackExtractorService.GetSubscriptionPeriod(callback.Data!);
        
        var subscription = await subscriptionRepository.GetActiveByTypeAndPeriodAsync(
            subscriptionType, subscriptionPeriod);

        if (subscription.Data is null)
        {
            await botService.SendTextMessageAsync(
                chatId,
                "No available subscription was founded.\nPlease contact @nazikBro",
                cancellationToken);
        }

        return Result.Success(subscription.Data!);
    }
}
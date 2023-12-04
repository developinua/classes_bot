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

public class SubscriptionService : ISubscriptionService
{
    private readonly IBotService _botService;
    private readonly ICallbackExtractorService _callbackExtractorService;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(
        IBotService botService,
        ICallbackExtractorService callbackExtractorService,
        ISubscriptionRepository subscriptionRepository)
    {
        _botService = botService;
        _callbackExtractorService = callbackExtractorService;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Result<Subscription>> GetSubscriptionFromCallback(
        CallbackQuery callback,
        long chatId,
        CancellationToken cancellationToken)
    {
        var subscriptionType = _callbackExtractorService.GetSubscriptionType(callback.Data!);
        var subscriptionPeriod = _callbackExtractorService.GetSubscriptionPeriod(callback.Data!);
        
        var subscription = await _subscriptionRepository.GetActiveByTypeAndPeriodAsync(
            subscriptionType, subscriptionPeriod);

        if (subscription.Data is null)
        {
            await _botService.SendTextMessageAsync(
                chatId,
                "No available subscription was founded.\nPlease contact @nazikBro",
                cancellationToken);
        }

        return Result.Success(subscription.Data!);
    }
}
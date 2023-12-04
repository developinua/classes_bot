using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Models.Enums;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Subscriptions;

public class SubscriptionsCallbackHandler : IRequestHandler<SubscriptionsCallbackRequest, Result>
{
    private readonly IBotService _botService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IReplyMarkupService _replyMarkupService;
    private readonly ICallbackExtractorService _callbackExtractorService;
    private readonly IValidator<CallbackQuery> _validator;

    public SubscriptionsCallbackHandler(
        IBotService botService,
        ISubscriptionService subscriptionService,
        IReplyMarkupService replyMarkupService,
        ICallbackExtractorService callbackExtractorService,
        IValidator<CallbackQuery> validator)
    {
        _botService = botService;
        _subscriptionService = subscriptionService;
        _replyMarkupService = replyMarkupService;
        _callbackExtractorService = callbackExtractorService;
        _validator = validator;
    }

    public async Task<Result> Handle(SubscriptionsCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await _validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            Result.Failure().WithMessage("No valid callback query.");

        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);
        
        var callbackType = _callbackExtractorService.GetSubscriptionCallbackQueryType(request.CallbackQuery.Data!);

        return callbackType switch
        {
            SubscriptionsCallbackQueryType.Group =>
                await SendSubscriptionGroupTextMessage(request, cancellationToken),
            SubscriptionsCallbackQueryType.Period =>
                await SendSubscriptionPeriodTextMessage(request, cancellationToken),
            _ => Result.Failure().WithMessage("Invalid subscription callback query type.")
        };
    }

    private async Task<Result> SendSubscriptionGroupTextMessage(
        SubscriptionsCallbackRequest request,
        CancellationToken cancellationToken)
    {
        await _botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            "*Which subscription period do you prefer?\n*",
            replyMarkup: _replyMarkupService.GetSubscriptionPeriods(request.CallbackQuery.Data!),
            cancellationToken: cancellationToken);
        return Result.Success();
    }
    
    private async Task<Result> SendSubscriptionPeriodTextMessage(
        SubscriptionsCallbackRequest request,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionService.GetSubscriptionFromCallback(
            request.CallbackQuery, request.ChatId, cancellationToken);
        
        if (subscription.IsFailure())
            return subscription;

        await _botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            $"*Price: {subscription.Data.GetPriceWithDiscount()}\n" +
            $"*P.S. Please send your username and subscription in comment",
            _replyMarkupService.GetBuySubscription(subscription.Data.Id),
            cancellationToken);
        await _botService.SendTextMessageAsync(
            request.ChatId,
            "*After your subscription will be approved by teacher\nYou will be able to /checkin on classes.*",
            cancellationToken);
        
        return Result.Success();
    }
}
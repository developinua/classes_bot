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

public class SubscriptionsCallbackHandler(
        IBotService botService,
        ISubscriptionService subscriptionService,
        IReplyMarkupService replyMarkupService,
        ICallbackExtractorService callbackExtractorService,
        IValidator<CallbackQuery> validator)
    : IRequestHandler<SubscriptionsCallbackRequest, Result>
{
    public async Task<Result> Handle(SubscriptionsCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            Result.Failure().WithMessage("No valid callback query.");

        await botService.SendChatActionAsync(request.ChatId, cancellationToken);
        
        var callbackType = callbackExtractorService.GetSubscriptionCallbackQueryType(request.CallbackQuery.Data!);

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
        await botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            "*Which subscription period do you prefer?\n*",
            replyMarkup: replyMarkupService.GetSubscriptionPeriods(request.CallbackQuery.Data!),
            cancellationToken: cancellationToken);
        return Result.Success();
    }
    
    private async Task<Result> SendSubscriptionPeriodTextMessage(
        SubscriptionsCallbackRequest request,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionService.GetSubscriptionFromCallback(
            request.CallbackQuery, request.ChatId, cancellationToken);
        
        if (subscription.IsFailure()) return subscription;

        await botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            $"*Price: {subscription.Data.GetPriceWithDiscount()}\n" +
            $"*P.S. Please send your username and subscription in comment",
            replyMarkupService.GetBuySubscription(subscription.Data.Id),
            cancellationToken);
        await botService.SendTextMessageAsync(
            request.ChatId,
            "*After your subscription will be approved by teacher\nYou will be able to /checkin on classes.*",
            cancellationToken);
        
        return Result.Success();
    }
}
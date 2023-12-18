using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Extensions;
using Classes.Application.Services;
using Classes.Domain.Models.Enums;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Subscriptions;

public class SubscriptionsCallbackHandler(
        IBotService botService,
        ISubscriptionService subscriptionService,
        IReplyMarkupService replyMarkupService,
        ICallbackExtractorService callbackExtractorService,
        IStringLocalizer<SubscriptionsHandler> localizer,
        IValidator<CallbackQuery> validator)
    : IRequestHandler<SubscriptionsCallbackRequest, Result>
{
    public async Task<Result> Handle(SubscriptionsCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            Result.Failure().WithMessage("No valid callback query.");

        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);
        
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
        botService.UseChat(request.ChatId);
        await botService.SendTextMessageWithReplyAsync(
            localizer.GetString("ChooseSubscriptionPeriod").WithNewLines(),
            replyMarkup: replyMarkupService.GetSubscriptionPeriods(request.CallbackQuery.Data!),
            cancellationToken: cancellationToken);
        return Result.Success();
    }
    
    private async Task<Result> SendSubscriptionPeriodTextMessage(
        SubscriptionsCallbackRequest request,
        CancellationToken cancellationToken)
    {
        botService.UseChat(request.ChatId);
        
        var type = callbackExtractorService.GetSubscriptionType(request.CallbackQuery.Data!);
        var period = callbackExtractorService.GetSubscriptionPeriod(request.CallbackQuery.Data!);
        var subscription = await subscriptionService.GetSubscriptionByTypeAndPeriod(type, period);
        
        if (subscription.IsFailure()) return subscription;
        
        if (subscription.Data is null)
        {
            await botService.SendTextMessageAsync(
                localizer.GetString("NoAvailableSubscriptions").WithNewLines(),
                cancellationToken);
            return Result.Failure().WithMessage("No available subscriptions were founded.");
        }

        await botService.SendTextMessageWithReplyAsync(
            localizer["BuySubscription", subscription.Data.GetPriceWithDiscount()].WithNewLines(),
            replyMarkupService.GetBuySubscription(subscription.Data.Id),
            cancellationToken);
        await botService.SendTextMessageAsync(
            localizer.GetString("BuySubscriptionApproval").WithNewLines(),
            cancellationToken);
        
        return Result.Success();
    }
}
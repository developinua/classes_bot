using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Subscriptions;

public class SubscriptionsDetailedInformationCallbackHandler(
        IBotService botService,
        IUserSubscriptionService userSubscriptionService,
        IReplyMarkupService replyMarkupService,
        ICallbackExtractorService callbackExtractorService,
        IStringLocalizer<SubscriptionsHandler> localizer,
        IValidator<CallbackQuery> validator)
    : IRequestHandler<SubscriptionsDetailedInformationCallbackRequest, Result>
{
    public async Task<Result> Handle(
        SubscriptionsDetailedInformationCallbackRequest request,
        CancellationToken cancellationToken)
    {
        if ((await validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            Result.Failure().WithMessage("No valid callback query.");

        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);
        
        var userSubscriptionId = callbackExtractorService.GetUserSubscriptionId(
            request.CallbackQuery.Data!, request.CallbackPattern);
        var userSubscription = await userSubscriptionService.GetById(userSubscriptionId);

        if (userSubscription.IsFailure() || userSubscription.Data is null)
        {
            await botService.SendTextMessageAsync(localizer.GetString("NoSubscriptions"),cancellationToken);
            return Result.Success();
        }

        // todo: add delete previous saved information message
        await botService.SendTextMessageWithReplyAsync(
            userSubscriptionService.GetUserSubscriptionInformation(userSubscription.Data),
            replyMarkup: replyMarkupService.GetBackToSubscriptions(userSubscription.Data),
            cancellationToken: cancellationToken);
        // todo: add save message when press any of choice button to delete it next
        
        return Result.Success();
    }
}
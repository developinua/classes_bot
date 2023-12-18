using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Extensions;
using Classes.Application.Services;
using Classes.Domain.Models;
using Classes.Domain.Requests;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;

namespace Classes.Application.Handlers.Subscriptions;

public class SubscriptionsHandler(
        IBotService botService,
        IReplyMarkupService replyMarkupService,
        IStringLocalizer<SubscriptionsHandler> localizer,
        IUserSubscriptionService userSubscriptionService)
    : IRequestHandler<SubscriptionsRequest, Result>
{
    public async Task<Result> Handle(SubscriptionsRequest request, CancellationToken cancellationToken)
    {
        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);
        
        var userSubscriptions = await userSubscriptionService.GetAllActiveWithRemainingClasses(request.Username);

        if (!userSubscriptions.Data.Any())
        {
            await botService.SendTextMessageWithReplyAsync(
                localizer.GetString("ChooseSubscription").WithNewLines(),
                replyMarkup: replyMarkupService.GetSubscriptions(),
                cancellationToken: cancellationToken);
            return Result.Success();
        }

        await SendUserSubscriptions(request.ChatId, userSubscriptions.Data, cancellationToken);

        return Result.Success();
    }
    
    private async Task SendUserSubscriptions(
        long chatId,
        IReadOnlyCollection<UserSubscription> userSubscriptions,
        CancellationToken cancellationToken)
    {
        var message = userSubscriptions.Count > 1 
            ? localizer.GetString("YourSubscriptions")
            : localizer.GetString("YourSubscription");

        botService.UseChat(chatId);
        await botService.SendTextMessageAsync(message, cancellationToken);

        foreach (var userSubscription in userSubscriptions)
        {
            // todo: localize
            var replyMessage =
                $"{localizer["Subscription", localizer[userSubscription.Subscription.NameCode]]}\n" +
                $"{localizer["Description", localizer[userSubscription.Subscription.DescriptionCode]]}\n" +
                $"{localizer["SubscriptionType", localizer[userSubscription.Subscription.Type.DisplayName()]]}\n" +
                $"{localizer["RemainingClasses", localizer[userSubscription.RemainingClasses.ToString()]]}\n";
            await botService.SendTextMessageAsync(replyMessage, cancellationToken);
        }

        // TODO: add functionality for adding multiple subscriptions
        await botService.SendTextMessageAsync(localizer.GetString("DoYouWantToCheckin"), cancellationToken);
    }
}
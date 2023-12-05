using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Classes.Domain.Requests;
using MediatR;
using ResultNet;

namespace Classes.Application.Handlers.Subscriptions;

public class SubscriptionsHandler(
        IBotService botService,
        IReplyMarkupService replyMarkupService,
        IUserSubscriptionRepository userSubscriptionRepository)
    : IRequestHandler<SubscriptionsRequest, Result>
{
    public async Task<Result> Handle(SubscriptionsRequest request, CancellationToken cancellationToken)
    {
        await botService.SendChatActionAsync(request.ChatId, cancellationToken);
        
        var userSubscriptions = await userSubscriptionRepository.GetAllActiveWithRemainingClasses(request.Username);

        if (!userSubscriptions.Data.Any())
        {
            await botService.SendTextMessageWithReplyAsync(
                request.ChatId,
                "*Which subscription do you want choose?\n*",
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
        var pluralEnding = userSubscriptions.Count > 1 ? "s" : "";

        await botService.SendTextMessageAsync(
            chatId,
            $"*Your subscription{pluralEnding}:*",
            cancellationToken);

        foreach (var replyMessage in userSubscriptions.Select(RenderUserSubscriptionInformationText))
            await botService.SendTextMessageAsync(chatId, replyMessage, cancellationToken);

        // TODO: add functionality for adding multiple subscriptions
        await botService.SendTextMessageAsync(
            chatId,
            "*Do you want to /checkin class?*",
            cancellationToken);
        return;
        
        // todo: localize
        string RenderUserSubscriptionInformationText(UserSubscription userSubscription) =>
            $"Subscription: {userSubscription.Subscription.Name}\n" +
            $"Description: {userSubscription.Subscription.Description}\n" +
            $"SubscriptionType: {userSubscription.Subscription.Type}\n" +
            $"Remaining Classes: {userSubscription.RemainingClasses}\n";
    }
}
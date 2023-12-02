using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Domain.Services;
using Classes.Domain.Utils;
using MediatR;
using ResultNet;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Handlers.Subscriptions;

public class SubscriptionsHandler : IRequestHandler<SubscriptionsRequest, Result>
{
    private readonly IBotService _botService;
    private readonly IUserSubscriptionService _userSubscriptionService;

    public SubscriptionsHandler(
        IBotService botService,
        IUserSubscriptionService userSubscriptionService)
    {
        _botService = botService;
        _userSubscriptionService = userSubscriptionService;
    }

    public async Task<Result> Handle(SubscriptionsRequest request, CancellationToken cancellationToken)
    {
        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);
        
        var userSubscriptions = (await _userSubscriptionService.GetActiveUserSubscriptionsWithRemainingClasses(
            request.Username)).Data;

        if (!userSubscriptions.Any())
        {
            await GetNewUserSubscriptionInformation(request.ChatId, cancellationToken);
            return Result.Success();
        }

        await GetExistingUserSubscriptionInformation(request.ChatId, userSubscriptions, cancellationToken);

        return Result.Success();
    }
    
    private async Task GetNewUserSubscriptionInformation(long chatId, CancellationToken cancellationToken) =>
        await _botService.SendTextMessageWithReplyAsync(
            chatId,
            "*Which subscription do you want choose?\n*",
            replyMarkup: RenderSubscriptionGroups(),
            cancellationToken: cancellationToken);
    
    private async Task GetExistingUserSubscriptionInformation(
        long chatId,
        IReadOnlyCollection<UserSubscription> userSubscriptions,
        CancellationToken cancellationToken)
    {
        var pluralEnding = userSubscriptions.Count > 1 ? "s" : "";

        await _botService.SendTextMessageAsync(
            chatId,
            $"*Your subscription{pluralEnding}:*",
            cancellationToken);

        foreach (var replyMessage in userSubscriptions.Select(RenderUserSubscriptionInformationText))
            await _botService.SendTextMessageAsync(chatId, replyMessage, cancellationToken);

        // TODO: Change to add functionality for adding few subscriptions
        await _botService.SendTextMessageAsync(
            chatId,
            "*Do you want to /checkin class?*",
            cancellationToken);
    }
    
    private static string RenderUserSubscriptionInformationText(UserSubscription userSubscription) =>
        $"Subscription: {userSubscription.Subscription.Name}\n" +
        $"Description: {userSubscription.Subscription.Description}\n" +
        $"SubscriptionType: {userSubscription.Subscription.Type}\n" +
        $"Remaining Classes: {userSubscription.RemainingClasses}\n";

    private static InlineKeyboardMarkup RenderSubscriptionGroups() =>
        InlineKeyboardBuilder.Create()
            .AddButton("Novice subscription", "subsGroup:novice").NewLine()
            .AddButton("Medium subscription", "subsGroup:medium").NewLine()
            .AddButton("Lady style subscription", "subsGroup:lady").NewLine()
            .AddButton("Novice and medium subscription", "subsGroup:novice-medium").NewLine()
            .AddButton("Novice and lady style subscription", "subsGroup:novice-lady").NewLine()
            .AddButton("Medium and lady style subscription", "subsGroup:medium-lady").NewLine()
            .AddButton("Premium", "subsGroup:premium")
            .Build();
}
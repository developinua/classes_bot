using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Data.Repositories;
using Classes.Domain.Utils;
using ResultNet;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Services;

public interface IUserSubscriptionService
{
    Task<Result> Update(UserSubscription userSubscription);
    Task<Result<UserSubscription?>> GetById(long id);
    Task ShowUserSubscriptionsInformation(long chatId, string username, CancellationToken cancellationToken);
}

public class UserSubscriptionService : IUserSubscriptionService
{
    private readonly IBotService _botService;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;

    public UserSubscriptionService(
        IBotService botService,
        IUserSubscriptionRepository userSubscriptionRepository)
    {
        _botService = botService;
        _userSubscriptionRepository = userSubscriptionRepository;
    }

    public async Task<Result> Update(UserSubscription userSubscription)
    {
        await _userSubscriptionRepository.Update(userSubscription);
        return Result.Success();
    }

    public async Task<Result<UserSubscription?>> GetById(long id)
    {
        var userSubscription = await _userSubscriptionRepository.GetById(id);
        return Result.Success(userSubscription);
    }

    public async Task ShowUserSubscriptionsInformation(
        long chatId,
        string username,
        CancellationToken cancellationToken)
    {
        var userSubscriptions = (await _userSubscriptionRepository.GetByUsername(username)).Data;
        
        // TODO: Check location where classes can be executed
        
        await SendSubscriptionTitle(chatId, userSubscriptions, cancellationToken);
        
        foreach (var userSubscription in userSubscriptions)
            await SendSubscriptionDetails(chatId, userSubscription, cancellationToken);

        await SendSubscriptionFooter(chatId, userSubscriptions, cancellationToken);
    }
    
    private async Task SendSubscriptionTitle(
        long chatId,
        List<UserSubscription> userSubscriptions,
        CancellationToken cancellationToken)
    {
        var replyMessage = userSubscriptions.Any()
            ? userSubscriptions.Count == 1 ? "*Your subscription:*" : "*Your subscriptions:*"
            : "You have no subscriptions. Press /subscriptions to buy one.";

        await _botService.SendTextMessageWithReplyAsync(
            chatId,
            replyMessage,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }

    private async Task SendSubscriptionDetails(
        long chatId,
        UserSubscription userSubscription,
        CancellationToken cancellationToken)
    {
        var replyText =
            "*Subscription:\n*" +
            $"Name: {userSubscription.Subscription.Name}\n" +
            $"SubscriptionType: {userSubscription.Subscription.Type}\n" +
            $"Remaining Classes: {userSubscription.RemainingClasses}\n";
        var replyMarkup = InlineKeyboardBuilder.Create()
            .AddButton("Check-in", $"check-in-subscription-id:{userSubscription.Id}")
            .Build();

        await _botService.SendTextMessageWithReplyAsync(chatId, replyText, replyMarkup, cancellationToken);
    }
    
    private async Task SendSubscriptionFooter(
        long chatId,
        IEnumerable<UserSubscription> userSubscriptions,
        CancellationToken cancellationToken)
    {
        if (!userSubscriptions.Any()) return;
        
        await _botService.SendTextMessageAsync(
            chatId,
            "*Press check-in button on the subscription where you want the class to be taken from*",
            cancellationToken);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Classes.Data.Repositories;
using Classes.Domain.Extensions;
using Classes.Domain.Utils;
using ResultNet;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Services;

public interface IUserSubscriptionService
{
    Task<Result> Update(UserSubscription userSubscription);
    Task<Result<UserSubscription?>> GetById(long id);
    Task<Result<IReadOnlyCollection<UserSubscription>>> GetByUsername(string username);
    Task<Result<UserSubscription?>> GetByUsernameAndType(string username, SubscriptionType subscriptionType);
    Task<Result<IReadOnlyCollection<UserSubscription>>> GetActiveUserSubscriptionsWithRemainingClasses(string username);
    Task ShowUserSubscriptionsInformation(long chatId, string username, CancellationToken cancellationToken);
    Task Add(UserSubscription userSubscription);
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

    public async Task<Result<IReadOnlyCollection<UserSubscription>>> GetByUsername(string username)
    {
        var userSubscriptions = await _userSubscriptionRepository.GetByUsername(username);
        return Result.Success(userSubscriptions.Data.AsReadOnlyCollection());
    }

    public async Task<Result<UserSubscription?>> GetByUsernameAndType(string username, SubscriptionType subscriptionType)
    {
        var userSubscription = await _userSubscriptionRepository.GetByUsernameAndType(username, subscriptionType);
        return Result.Success(userSubscription.Data);
    }

    public async Task<Result<IReadOnlyCollection<UserSubscription>>> GetActiveUserSubscriptionsWithRemainingClasses(
        string username)
    {
        var userSubscriptions = await _userSubscriptionRepository.GetActiveUserSubscriptionsWithRemainingClasses(
            username);
        return Result.Success(userSubscriptions.Data.AsReadOnlyCollection());
    }

    public async Task ShowUserSubscriptionsInformation(
        long chatId,
        string username,
        CancellationToken cancellationToken)
    {
        var userSubscriptions = (await GetByUsername(username)).Data;
        
        // TODO: Check location where classes can be executed
        
        await SendSubscriptionTitle(chatId, userSubscriptions, cancellationToken);
        
        foreach (var userSubscription in userSubscriptions)
            await SendSubscriptionDetails(chatId, userSubscription, cancellationToken);

        await SendSubscriptionFooter(chatId, userSubscriptions, cancellationToken);
    }

    public async Task Add(UserSubscription userSubscription) =>
        await _userSubscriptionRepository.Add(userSubscription);

    private async Task SendSubscriptionTitle(
        long chatId,
        IReadOnlyCollection<UserSubscription> userSubscriptions,
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
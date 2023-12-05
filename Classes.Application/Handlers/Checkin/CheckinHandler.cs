using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Application.Utils;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Application.Handlers.Checkin;

public class CheckinHandler(
        IBotService botService,
        IUserSubscriptionRepository userSubscriptionRepository,
        IValidator<Message> locationValidator)
    : IRequestHandler<CheckinRequest, Result>
{
    public async Task<Result> Handle(CheckinRequest request, CancellationToken cancellationToken)
    {
        await botService.SendChatActionAsync(request.ChatId, cancellationToken);

        var isLocationDataValidationResult = await locationValidator.ValidateAsync(request.Message, cancellationToken);

        if (isLocationDataValidationResult.IsValid)
            await ShowUserSubscriptionsInformation(request.ChatId, request.Username, cancellationToken);

        var replyMarkup = new ReplyKeyboardMarkup(KeyboardButton.WithRequestLocation("Send location"))
        {
            OneTimeKeyboard = true,
            ResizeKeyboard = true
        };

        await botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            "Please send me your location, so I can check if you are on classes now.",
            replyMarkup,
            cancellationToken);
        
        return Result.Success();
    }
    
    private async Task ShowUserSubscriptionsInformation(
        long chatId,
        string username,
        CancellationToken cancellationToken)
    {
        var userSubscriptions = (await userSubscriptionRepository.GetByUsername(username)).Data;
        
        // TODO: Check location where classes can be executed
        
        await SendSubscriptionTitle();
        
        foreach (var userSubscription in userSubscriptions)
            await SendSubscriptionDetails(userSubscription);

        await SendSubscriptionFooter();

        return;
        
        async Task SendSubscriptionTitle()
        {
            var replyMessage = userSubscriptions.Any()
                ? userSubscriptions.Count == 1 ? "*Your subscription:*" : "*Your subscriptions:*"
                : "You have no subscriptions. Press /subscriptions to buy one.";

            await botService.SendTextMessageWithReplyAsync(
                chatId,
                replyMessage,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        async Task SendSubscriptionDetails(UserSubscription userSubscription)
        {
            var replyText =
                "*Subscription:\n*" +
                $"Name: {userSubscription.Subscription.Name}\n" +
                $"SubscriptionType: {userSubscription.Subscription.Type}\n" +
                $"Remaining Classes: {userSubscription.RemainingClasses}\n";
            var replyMarkup = InlineKeyboardBuilder.Create()
                .AddButton("Check-in", $"check-in-subscription-id:{userSubscription.Id}")
                .Build();

            await botService.SendTextMessageWithReplyAsync(chatId, replyText, replyMarkup, cancellationToken);
        }
    
        async Task SendSubscriptionFooter()
        {
            if (!userSubscriptions.Any()) return;
        
            await botService.SendTextMessageAsync(
                chatId,
                "*Press check-in button on the subscription where you want the class to be taken from*",
                cancellationToken);
        }
    }
}
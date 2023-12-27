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
using Microsoft.Extensions.Localization;
using ResultNet;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Application.Handlers.Checkin;

public class CheckinHandler(
        IBotService botService,
        IStringLocalizer<CheckinHandler> localizer,
        IUserSubscriptionRepository userSubscriptionRepository,
        IValidator<Message> locationValidator)
    : IRequestHandler<CheckinRequest, Result>
{
    public async Task<Result> Handle(CheckinRequest request, CancellationToken cancellationToken)
    {
        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);

        var isLocationDataValidationResult = await locationValidator.ValidateAsync(request.Message, cancellationToken);

        if (isLocationDataValidationResult.IsValid)
            await ShowUserSubscriptionsInformation(request.Username, cancellationToken);

        var replyMarkup = new ReplyKeyboardMarkup(KeyboardButton.WithRequestLocation(
            localizer.GetString("SendLocationRequest")))
        {
            OneTimeKeyboard = true,
            ResizeKeyboard = true
        };

        await botService.SendTextMessageWithReplyAsync(
            localizer.GetString("SendLocation"), replyMarkup, cancellationToken);
        
        return Result.Success();
    }
    
    private async Task ShowUserSubscriptionsInformation(string username, CancellationToken cancellationToken)
    {
        var userSubscriptions = await userSubscriptionRepository.GetByUsername(username);
        
        // TODO: Check location where classes can be executed
        
        await SendSubscriptionTitle();
        
        foreach (var userSubscription in userSubscriptions.Data)
            await SendSubscriptionDetails(userSubscription);

        await SendSubscriptionFooter();

        return;
        
        async Task SendSubscriptionTitle()
        {
            // todo: localize
            var replyMessage = userSubscriptions.Data.Any()
                ? userSubscriptions.Data.Count == 1 ? "*Your subscription:*" : "*Your subscriptions:*"
                : "You have no subscriptions\\. \n Press /subscriptions to buy one\\.";

            await botService.SendTextMessageWithReplyAsync(
                replyMessage,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        async Task SendSubscriptionDetails(UserSubscription userSubscription)
        {
            // todo: localize
            var replyText =
                "*Subscription:\n*" +
                $"Name: {userSubscription.Subscription.Name}\n" +
                $"SubscriptionType: {userSubscription.Subscription.Type}\n" +
                $"Remaining Classes: {userSubscription.RemainingClasses}\n";
            var replyMarkup = InlineKeyboardBuilder.Create()
                .AddButton("Check-in", $"check-in-subscription-id:{userSubscription.Id}")
                .Build();

            await botService.SendTextMessageWithReplyAsync(replyText, replyMarkup, cancellationToken);
        }
    
        async Task SendSubscriptionFooter()
        {
            if (!userSubscriptions.Data.Any()) return;
        
            // todo: localize
            await botService.SendTextMessageAsync(
                "*Press checkin button on the subscription where you want the class to be taken from*",
                cancellationToken);
        }
    }
}
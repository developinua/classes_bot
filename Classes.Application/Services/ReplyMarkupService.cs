using System.Collections.Generic;
using Classes.Application.Handlers.Subscriptions;
using Classes.Application.Utils;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Application.Services;

public interface IReplyMarkupService
{
    IReplyMarkup GetStartMarkup();
    IReplyMarkup GetSubscriptions();
    IReplyMarkup GetBackToSubscriptions(UserSubscription userSubscription);
    IReplyMarkup GetSubscriptionsInformation(IReadOnlyCollection<UserSubscription> userSubscriptions);
    IReplyMarkup GetBuySubscription(long subscriptionId);
}

public class ReplyMarkupService(IStringLocalizer<SubscriptionsHandler> localizer) : IReplyMarkupService
{
    public IReplyMarkup GetStartMarkup()
    {
        var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
            .AddButton("English", "language:en-US")
            .AddButton("Українська", "language:uk-UA");
        return replyKeyboardMarkup.Build();
    }

    public IReplyMarkup GetSubscriptions()
    {
        var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
            .AddButton(localizer["SubscriptionClasses"], "subscription:class").NewLine()
            .AddButton(localizer["SubscriptionCourses"], "subscription:course");
        return replyKeyboardMarkup.Build();
    }

    public IReplyMarkup GetBackToSubscriptions(UserSubscription userSubscription)
    {
        var type = userSubscription.Subscription.Type == SubscriptionType.Class ? "class" : "course";
        var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
            .AddButton(localizer["BackToSubscriptions"], $"subscription:{type}");

        return replyKeyboardMarkup.Build();
    }

    public IReplyMarkup GetSubscriptionsInformation(IReadOnlyCollection<UserSubscription> userSubscriptions)
    {
        var replyKeyboardMarkup = InlineKeyboardBuilder.Create();

        foreach (var userSubscription in userSubscriptions)
        {
            // todo: get localized name from the Localization db
            var name = $"{userSubscription.Subscription.Name} ({userSubscription.RemainingClasses})";
            replyKeyboardMarkup.AddButton(name, $"user-subscription-id:{userSubscription.Id}").NewLine();
        }

        return replyKeyboardMarkup.Build();
    }

    public IReplyMarkup GetBuySubscription(long subscriptionId)
    {
        // todo: replacement link
        return InlineKeyboardBuilder.Create()
            .AddUrlButton("Buy", $"paymentlink:{subscriptionId}", "")
            .Build();
    }
}
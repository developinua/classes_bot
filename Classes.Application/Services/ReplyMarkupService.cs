using System.Collections.Generic;
using Classes.Application.Utils;
using Classes.Domain.Models;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Application.Services;

public interface IReplyMarkupService
{
    IReplyMarkup GetStartMarkup();
    IReplyMarkup GetSubscriptions();
    IReplyMarkup GetSubscriptionsInformation(IReadOnlyCollection<UserSubscription> userSubscriptions);
    IReplyMarkup GetSubscriptionPeriods(string subscriptionGroup);
    IReplyMarkup GetBuySubscription(long subscriptionId);
}

public class ReplyMarkupService(IStringLocalizer<ReplyMarkupService> localizer) : IReplyMarkupService
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
            .AddButton(localizer["SubscriptionClasses"], "subscription:class")
            .AddButton(localizer["SubscriptionCourses"], "subscription:course");
        return replyKeyboardMarkup.Build();
    }

    public IReplyMarkup GetSubscriptionsInformation(IReadOnlyCollection<UserSubscription> userSubscriptions)
    {
        var replyKeyboardMarkup = InlineKeyboardBuilder.Create();

        foreach (var userSubscription in userSubscriptions)
        {
            // todo: get localized name from the Localization db
            var name = localizer[
                "SubscriptionName",
                userSubscription.Subscription.Name,
                userSubscription.RemainingClasses];
            replyKeyboardMarkup.AddButton(name, $"user-subscription:{userSubscription.Id}").NewLine();
        }

        return replyKeyboardMarkup.Build();
    }

    public IReplyMarkup GetSubscriptionPeriods(string subscriptionGroup)
    {
        var subscriptionPeriods = InlineKeyboardBuilder.Create();

        if (!subscriptionGroup.Contains("Premium"))
            subscriptionPeriods.AddButton("Day", $"{subscriptionGroup}?subsPeriod:day").NewLine();

        subscriptionPeriods.AddButton("Week", $"{subscriptionGroup}?subsPeriod:week").NewLine()
            .AddButton("Two weeks", $"{subscriptionGroup}?subsPeriod:two-weeks").NewLine()
            .AddButton("Month", $"{subscriptionGroup}?subsPeriod:month").NewLine()
            .AddButton("Three months", $"{subscriptionGroup}?subsPeriod:three-months");

        return subscriptionPeriods.Build();
    }

    public IReplyMarkup GetBuySubscription(long subscriptionId)
    {
        // todo: replacement link
        return InlineKeyboardBuilder.Create()
            .AddUrlButton("Buy", $"paymentlink:{subscriptionId}", "")
            .Build();
    }
}
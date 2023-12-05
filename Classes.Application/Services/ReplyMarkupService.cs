using Classes.Application.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Application.Services;

public interface IReplyMarkupService
{
    IReplyMarkup GetStartMarkup();
    IReplyMarkup GetSubscriptions();
    IReplyMarkup GetSubscriptionPeriods(string subscriptionGroup);
    IReplyMarkup GetBuySubscription(long subscriptionId);
}

public class ReplyMarkupService(ILogger<ReplyMarkupService> logger) : IReplyMarkupService
{
    public IReplyMarkup GetStartMarkup()
    {
        var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
            .AddButton("English", "language:en-US")
            .AddButton("Ukrainian", "language:uk-UA")
            .Build();
        return replyKeyboardMarkup;
    }

    public IReplyMarkup GetSubscriptions()
    {
        var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
            .AddButton("Novice subscription", "subsGroup:novice").NewLine()
            .AddButton("Medium subscription", "subsGroup:medium").NewLine()
            .AddButton("Lady style subscription", "subsGroup:lady").NewLine()
            .AddButton("Novice and medium subscription", "subsGroup:novice-medium").NewLine()
            .AddButton("Novice and lady style subscription", "subsGroup:novice-lady").NewLine()
            .AddButton("Medium and lady style subscription", "subsGroup:medium-lady").NewLine()
            .AddButton("Premium", "subsGroup:premium")
            .Build();
        return replyKeyboardMarkup;
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
            .AddUrlButton("Buy", $"paymentlink:{subscriptionId}", "https://send.monobank.ua/4aXrhJ1FTH")
            .Build();
    }
}
using Classes.Application.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Application.Services;

public interface IReplyMarkupService
{
    IReplyMarkup GetStartMarkup();
    IReplyMarkup GetSubscriptions();
    IReplyMarkup GetSubscriptionPeriods(string subscriptionGroupData);
    IReplyMarkup GetBuySubscription(long subscriptionId);
}

public class ReplyMarkupService : IReplyMarkupService
{
    private readonly ILogger<ReplyMarkupService> _logger;

    public ReplyMarkupService(ILogger<ReplyMarkupService> logger) => _logger = logger;

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
    
    private static InlineKeyboardMarkup RenderSubscriptionPeriods(string subscriptionGroupData)
    {
        var subscriptionPeriods = InlineKeyboardBuilder.Create();

        if (!subscriptionGroupData.Contains("Premium"))
            subscriptionPeriods.AddButton("Day", $"{subscriptionGroupData}?subsPeriod:day").NewLine();

        subscriptionPeriods.AddButton("Week", $"{subscriptionGroupData}?subsPeriod:week").NewLine()
            .AddButton("Two weeks", $"{subscriptionGroupData}?subsPeriod:two-weeks").NewLine()
            .AddButton("Month", $"{subscriptionGroupData}?subsPeriod:month").NewLine()
            .AddButton("Three months", $"{subscriptionGroupData}?subsPeriod:three-months");

        return subscriptionPeriods.Build();
    }
    
    public IReplyMarkup GetSubscriptionPeriods(string subscriptionGroupData)
    {
        var subscriptionPeriods = InlineKeyboardBuilder.Create();

        if (!subscriptionGroupData.Contains("Premium"))
            subscriptionPeriods.AddButton("Day", $"{subscriptionGroupData}?subsPeriod:day").NewLine();

        subscriptionPeriods.AddButton("Week", $"{subscriptionGroupData}?subsPeriod:week").NewLine()
            .AddButton("Two weeks", $"{subscriptionGroupData}?subsPeriod:two-weeks").NewLine()
            .AddButton("Month", $"{subscriptionGroupData}?subsPeriod:month").NewLine()
            .AddButton("Three months", $"{subscriptionGroupData}?subsPeriod:three-months");

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
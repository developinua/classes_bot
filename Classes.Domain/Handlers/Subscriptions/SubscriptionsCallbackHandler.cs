using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Classes.Data.Models.Enums;
using Classes.Domain.Models.Enums;
using Classes.Domain.Services;
using Classes.Domain.Utils;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Handlers.Subscriptions;

public class SubscriptionsCallbackHandler : IRequestHandler<SubscriptionsCallbackRequest, Result>
{
    private readonly IBotService _botService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IValidator<CallbackQuery> _validator;

    public SubscriptionsCallbackHandler(
        IBotService botService,
        ISubscriptionService subscriptionService,
        IValidator<CallbackQuery> validator)
    {
        _botService = botService;
        _subscriptionService = subscriptionService;
        _validator = validator;
    }

    public async Task<Result> Handle(SubscriptionsCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await _validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            throw new NotSupportedException();

        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);
        
        return await ParseSubscription(request, cancellationToken);
    }
    
    private async Task<Result> ParseSubscription(
        SubscriptionsCallbackRequest request,
        CancellationToken cancellationToken)
    {
        var subscriptionCallbackQueryType = GetSubscriptionCallbackQueryType(request.CallbackQuery.Data!);

        var temp = subscriptionCallbackQueryType switch
        {
            MySubscriptionsCallbackQueryType.Group =>
                SendSubscriptionGroupTextMessage(request, cancellationToken),
            MySubscriptionsCallbackQueryType.Period =>
                await SendSubscriptionPeriodTextMessage(request, cancellationToken),
            _ => null
        };

        if (temp is null)
        {
            return Result.Failure().WithMessage("No such subscription callback query type was founded.");
        }

        await temp.Invoke();
        return Result.Success();
    }
    
    private static MySubscriptionsCallbackQueryType GetSubscriptionCallbackQueryType(string callbackQueryData) =>
        callbackQueryData switch
        {
            _ when callbackQueryData.Contains("subsGroup") && !callbackQueryData.Contains("subsPeriod") =>
                MySubscriptionsCallbackQueryType.Group,
            _ when callbackQueryData.Contains("subsGroup") && callbackQueryData.Contains("subsPeriod") =>
                MySubscriptionsCallbackQueryType.Period,
            _ => throw new InvalidDataContractException(
                "Can't parse subscription group from user callback query data.")
        };
    
    private Func<Task<Message>> SendSubscriptionGroupTextMessage(
        SubscriptionsCallbackRequest request,
        CancellationToken cancellationToken)
    {
        var replyKeyboardMarkup = RenderSubscriptionPeriods(request.CallbackQuery.Data!);
        return () => _botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            "*Which subscription period do you prefer?\n*",
            replyMarkup: replyKeyboardMarkup,
            cancellationToken: cancellationToken);
    }
    
    private async Task<Func<Task<Message>>> SendSubscriptionPeriodTextMessage(
        SubscriptionsCallbackRequest request,
        CancellationToken cancellationToken)
    {
        Enum.TryParse(GetSubscriptionGroupData(request.CallbackQuery.Data!), true, out SubscriptionType subsGroup);
        Enum.TryParse(GetSubscriptionPeriodData(request.CallbackQuery.Data!), true, out SubscriptionPeriod subsPeriod);
        
        var subscription = await _subscriptionService.GetActiveSubscriptionByTypeAndPeriodAsync(subsGroup, subsPeriod);

        if (subscription.Data is null)
        {
            return async () => await _botService.SendTextMessageAsync(
                request.ChatId,
                "No available subscription was founded.\nPlease contact @nazikBro",
                cancellationToken);
        }

        await _botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            $"*Price: {subscription.Data.GetPriceWithDiscount()}\n" +
            $"*P.S. Please send your username and subscription in comment",
            RenderBuySubscription(subscription.Data.Id),
            cancellationToken);

        return async () => await _botService.SendTextMessageAsync(
            request.ChatId,
            "*After your subscription will be approved by teacher\nYou will be able to /checkin on classes.*",
            cancellationToken);
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
    
    private static string GetSubscriptionGroupData(string callbackQueryData)
    {
        var subscriptionGroup = string.Empty;
        var subscriptionGroupMatch = Regex.Match(callbackQueryData, @"(?i)(?<query>subsGroup):(?<data>\w+)");
        var subscriptionGroupMatchQuery = subscriptionGroupMatch.Groups["query"].Value;
        var subscriptionGroupMatchData = subscriptionGroupMatch.Groups["data"].Value;

        if (subscriptionGroupMatch.Success && subscriptionGroupMatchQuery.Equals("subsGroup"))
            subscriptionGroup = subscriptionGroupMatchData;

        return subscriptionGroup;
    }

    private static string GetSubscriptionPeriodData(string callbackQueryData)
    {
        var subscriptionPeriod = string.Empty;
        var subscriptionPeriodMatch = Regex.Match(callbackQueryData, @"(?i)(?<query>subsPeriod):(?<data>\w+)");
        var subscriptionPeriodMatchQuery = subscriptionPeriodMatch.Groups["query"].Value;
        var subscriptionPeriodMatchData = subscriptionPeriodMatch.Groups["data"].Value;

        if (subscriptionPeriodMatch.Success && subscriptionPeriodMatchQuery.Equals("subsPeriod"))
            subscriptionPeriod = subscriptionPeriodMatchData;

        return subscriptionPeriod;
    }

    private static InlineKeyboardMarkup RenderBuySubscription(long id)
    {
        // TODO: Add replacement link
        return InlineKeyboardBuilder.Create()
            .AddUrlButton("Buy", $"paymentlink:{id}", "https://send.monobank.ua/4aXrhJ1FTH")
            .Build();
    }
}
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Validators;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Commands.MySubscriptions;

public class MySubscriptionsCommand : IBotCommand
{
    public string Name => "/mySubscriptions";
    public string CallbackQueryPattern => "(?i)(?<query>subsGroup|subsPeriod)";

    public bool Contains(Message message) => message.Type == MessageType.Text && message.Text!.Contains(Name);

    public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

    public async Task Execute(Message message, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        await client.SendChatActionAsync(message.From!.Id, ChatAction.Typing);
        await MySubscriptionsCommandHelper.GetUserSubscriptionInformation(message, client, dbContext);
    }

    public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        if (callbackQuery.Validate())
            throw new NotSupportedException();

        var chatId = callbackQuery.From.Id;
        await client.SendChatActionAsync(chatId, ChatAction.Typing);
        await MySubscriptionsCommandHelper.ParseSubscription(chatId, callbackQuery, client, dbContext);
    }
}
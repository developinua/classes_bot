using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Domain.Repositories;
using Classes.Domain.Validators;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Commands.MySubscriptions;

public class MySubscriptionsCommand : IBotCommand
{
    public string Name => @"/mysubscriptions";
    public string CallbackQueryPattern => @"(?i)(?<query>subs-group|subs-period)";

    public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

    public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

    public async Task Execute(Message message, ITelegramBotClient client, IUnitOfWork services)
    {
        await client.SendChatActionAsync(message.From.Id, ChatAction.Typing);
        await MySubscriptionsCommandHelper.GetUserSubscriptionInformation(message, client, services);
    }

    public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, IUnitOfWork services)
    {
        if (callbackQuery.Validate())
            throw new NotSupportedException();

        var chatId = callbackQuery.From.Id;
        await client.SendChatActionAsync(chatId, ChatAction.Typing);
        await MySubscriptionsCommandHelper.ParseSubscription(chatId, callbackQuery, client, services);
    }
}
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Domain.Repositories;
using Classes.Domain.Validators;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Commands.CheckIn;

public class CheckInCommand : IBotCommand
{
    public string Name => @"/check-in";
    public string CallbackQueryPattern => @"(?i)(?<query>check-in-subscription-id):(?<data>.*)";

    public bool Contains(Message message) => message.Type == MessageType.Text && message.Text!.Contains(Name);

    public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

    public async Task Execute(Message message, ITelegramBotClient client, IUnitOfWork services)
    {
        var chatId = message.Chat.Id;
        await client.SendChatActionAsync(chatId, ChatAction.Typing);

        var isLocationMessage = message.ValidateMessageLocationData();
        if (isLocationMessage)
        {
            // TODO: Check location where classes can be executed
            await CheckInCommandHelper.ShowUserSubscriptionsInformation(message, client, services);
            return;
        }

        const string replyMessage = "Please send me your location, so I can check if you are on classes now.";
        var replyMarkup = new ReplyKeyboardMarkup(KeyboardButton.WithRequestLocation("Send location"))
        {
            OneTimeKeyboard = true,
            ResizeKeyboard = true
        };

        await client.SendTextMessageAsync(chatId, replyMessage, ParseMode.Markdown, replyMarkup: replyMarkup);
    }

    public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, IUnitOfWork services)
    {
        if (callbackQuery.Validate())
            throw new NotSupportedException();

        var chatId = callbackQuery.From.Id;
        await client.SendChatActionAsync(chatId, ChatAction.Typing);

        var userSubscriptionId = CheckInCommandHelper.GetUserSubscriptionIdFromCallbackQuery(
            callbackQuery.Data!, CallbackQueryPattern);
        var userSubscription = await services.UsersSubscriptions.FindOneAsync(x => x.Id.Equals(userSubscriptionId));

        if (userSubscription == null)
        {
            await client.SendTextMessageAsync(chatId, 
                "Can't get user subscription from db. Please contact @nazikBro for details", 
                ParseMode.Markdown);
            return;
        }

        if (userSubscription.RemainingClassesCount == 0)
        {
            await client.SendTextMessageAsync(chatId, 
                "You haven't any available classes. Press /mySubscriptions to manage your subscriptions", 
                ParseMode.Markdown);
            return;
        }

        userSubscription.RemainingClassesCount--;
        await services.UsersSubscriptions.ReplaceAsync(userSubscription);

        await client.SendTextMessageAsync(chatId, "*💚*", ParseMode.Markdown);
    }
}
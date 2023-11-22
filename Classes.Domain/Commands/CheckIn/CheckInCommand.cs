using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Commands.CheckIn;

public class CheckInCommand : IBotCommand
{
    public string Name => "/check-in";
    public string CallbackQueryPattern => "(?i)(?<query>check-in-subscription-id):(?<data>.*)";
    
    private readonly IValidator<CallbackQuery> _callbackQueryValidator;
    private readonly IValidator<Message> _locationValidator;

    public CheckInCommand(
        IValidator<CallbackQuery> callbackQueryValidator,
        IValidator<Message> locationValidator) =>
        (_callbackQueryValidator, _locationValidator) = (callbackQueryValidator, locationValidator);

    public bool Contains(Message message) => message.Type == MessageType.Text && message.Text!.Contains(Name);

    public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

    public async Task Execute(Message message, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        var chatId = message.Chat.Id;
        await client.SendChatActionAsync(chatId, ChatAction.Typing);

        var isLocationDataValid = (await _locationValidator.ValidateAsync(message)).IsValid;

        if (isLocationDataValid)
        {
            // TODO: Check location where classes can be executed
            await CheckInCommandHelper.ShowUserSubscriptionsInformation(message, client, dbContext);
            return;
        }

        var replyMarkup = new ReplyKeyboardMarkup(KeyboardButton.WithRequestLocation("Send location"))
        {
            OneTimeKeyboard = true,
            ResizeKeyboard = true
        };

        await client.SendTextMessageAsync(
            chatId,
            "Please send me your location, so I can check if you are on classes now.",
            parseMode: ParseMode.Markdown,
            replyMarkup: replyMarkup);
    }

    public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        if ((await _callbackQueryValidator.ValidateAsync(callbackQuery)).IsValid)
            throw new NotSupportedException();

        var chatId = callbackQuery.From.Id;
        await client.SendChatActionAsync(chatId, ChatAction.Typing);

        var userSubscriptionId = CheckInCommandHelper.GetUserSubscriptionIdFromCallbackQuery(
            callbackQuery.Data!, CallbackQueryPattern);
        var userSubscription = await dbContext.UsersSubscriptions
            .FirstOrDefaultAsync(x => x.Id.Equals(userSubscriptionId));

        if (userSubscription == null)
        {
            await client.SendTextMessageAsync(
                chatId, 
                "Can't get user subscription from db. Please contact @nazikBro for details", 
                parseMode: ParseMode.Markdown);
            return;
        }

        if (userSubscription.RemainingClasses == 0)
        {
            await client.SendTextMessageAsync(
                chatId, 
                "You haven't any available classes. Press /mySubscriptions to manage your subscriptions", 
                parseMode: ParseMode.Markdown);
            return;
        }

        userSubscription.RemainingClasses--;
        dbContext.UsersSubscriptions.Update(userSubscription);
        
        await dbContext.SaveChangesAsync();
        await client.SendTextMessageAsync(chatId, "*💚*", parseMode: ParseMode.Markdown);
    }
}
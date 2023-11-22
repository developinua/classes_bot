using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Services;
using Classes.Domain.Utils;
using FluentValidation;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Commands.Start;

public class StartCommand : IBotCommand
{
    public string Name => "/start";
    public string CallbackQueryPattern => @"(?i)(?<query>language):(?<data>\w{2}-\w{2})";

    private readonly IUserService _userService;
    private readonly IValidator<CallbackQuery> _validator;

    public StartCommand(IUserService userService, IValidator<CallbackQuery> validator) =>
        (_userService, _validator) = (userService, validator);

    public bool Contains(Message message) => message.Type == MessageType.Text && message.Text!.Contains(Name);

    public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

    public async Task Execute(Message message, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

        if (string.IsNullOrEmpty(message.From!.Username!))
        {
            await client.SendTextMessageAsync(
                message.Chat.Id, "Fill in username and press /start again",
                parseMode: ParseMode.Markdown);
            return;
        }

        var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
            .AddButton("English", "language:en-US")
            .Build();

        await client.SendTextMessageAsync(
            message.Chat.Id,
            "*😊 Hi!\n\n*What language do you want to communicate in?",
            parseMode: ParseMode.Markdown,
            replyMarkup: replyKeyboardMarkup);
    }

    public async Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        if ((await _validator.ValidateAsync(callbackQuery)).IsValid)
            throw new NotSupportedException();

        var chatId = callbackQuery.From.Id;

        await client.SendChatActionAsync(chatId, ChatAction.Typing);
        await _userService.SaveUser(callbackQuery, CallbackQueryPattern);
        await client.SendTextMessageAsync(
            chatId,
            "*😊Successfully!😊*\nPress /mySubscriptions to manage your class subscription.",
            parseMode: ParseMode.Markdown);
    }
}
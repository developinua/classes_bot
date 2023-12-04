using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Application.Services;

public interface IBotService
{
    Task<Message> SendTextMessageAsync(
        long chatId,
        string text,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown);

    Task<Message> SendTextMessageWithReplyAsync(
        long chatId,
        string text,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown);

    Task SendChatActionAsync(
        long chatId,
        CancellationToken cancellationToken,
        ChatAction typing = ChatAction.Typing);
}

public class BotService : IBotService
{
    private readonly ITelegramBotClient _botClient;

    public BotService(ITelegramBotClient botClient) => _botClient = botClient;

    public async Task<Message> SendTextMessageAsync(
        long chatId,
        string text,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown)
    {
        return await _botClient.SendTextMessageAsync(
            chatId,
            text,
            parseMode: parseMode,
            cancellationToken: cancellationToken);
    }

    public async Task<Message> SendTextMessageWithReplyAsync(
        long chatId,
        string text,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown)
    {
        return await _botClient.SendTextMessageAsync(
            chatId,
            text,
            parseMode: parseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    public async Task SendChatActionAsync(
        long chatId,
        CancellationToken cancellationToken,
        ChatAction typing = ChatAction.Typing)
    {
        await _botClient.SendChatActionAsync(
            chatId,
            chatAction: typing,
            cancellationToken: cancellationToken);
    }
}
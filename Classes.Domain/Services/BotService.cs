using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Services;

public interface IBotService
{
    Task SendTextMessageAsync(
        long chatId,
        string text,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown);

    Task SendTextMessageWithReplyAsync(
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

    public async Task SendTextMessageAsync(
        long chatId,
        string text,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown)
    {
        await _botClient.SendTextMessageAsync(
            chatId,
            text,
            parseMode: parseMode,
            cancellationToken: cancellationToken);
    }

    public async Task SendTextMessageWithReplyAsync(
        long chatId,
        string text,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown)
    {
        await _botClient.SendTextMessageAsync(
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
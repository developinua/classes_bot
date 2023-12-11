using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Application.Services;

public interface IBotService
{
    void UseChat(long chatId);
    Task<Message> SendTextMessageAsync(
        string text,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown);
    Task<Message> SendTextMessageWithReplyAsync(
        string text,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown);
    Task SendChatActionAsync(CancellationToken cancellationToken, ChatAction typing = ChatAction.Typing);
}

public class BotService(ITelegramBotClient botClient) : IBotService
{
    private long ChatId { get; set; }

    public void UseChat(long chatId) => ChatId = chatId;

    public async Task<Message> SendTextMessageAsync(
        string text,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown)
    {
        return await botClient.SendTextMessageAsync(
            ChatId,
            text,
            parseMode: parseMode,
            cancellationToken: cancellationToken);
    }

    public async Task<Message> SendTextMessageWithReplyAsync(
        string text,
        IReplyMarkup replyMarkup,
        CancellationToken cancellationToken,
        ParseMode parseMode = ParseMode.Markdown)
    {
        return await botClient.SendTextMessageAsync(
            ChatId,
            text,
            parseMode: parseMode,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    public async Task SendChatActionAsync(
        CancellationToken cancellationToken,
        ChatAction typing = ChatAction.Typing)
    {
        await botClient.SendChatActionAsync(ChatId, chatAction: typing, cancellationToken: cancellationToken);
    }
}
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Context;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Commands.Administration;

public class AdminCommand : IBotCommand
{
    public string Name => "/admin";
    public string CallbackQueryPattern => "Not implemented";

    public bool Contains(Message message) => message.Type == MessageType.Text && message.Text!.Contains(Name);

    public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

    public async Task Execute(Message message, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        var chatId = message.From!.Id;

        if (!CanExecuteCommand(message.From.Username!))
            await client.SendTextMessageAsync(chatId, "Access denied. You can't execute this command.");

        var responseMessage = $"/seed /paymentlink /manage-subscriptions";
    }

    public Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, PostgresDbContext dbContext)
    {
        return Task.CompletedTask;
    }
    
    // todo: extract to separate class
    private static bool CanExecuteCommand(string username)
    {
        var allowedUsers = new[] { "nazikBro", "taras_zouk", "kovalinas" };
        return allowedUsers.Any(x => x.Equals(username));
    }
}
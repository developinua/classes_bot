using System.Threading.Tasks;
using Classes.Data.Context;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Commands;

public interface IBotCommand
{
    string Name { get; }
    string CallbackQueryPattern { get; }
    
    Task Execute(Message message, ITelegramBotClient client, PostgresDbContext dbContext);
    Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, PostgresDbContext dbContext);
    bool Contains(Message message);
    bool Contains(string callbackQueryData);
}
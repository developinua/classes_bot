using System.Threading.Tasks;
using Classes.Domain.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Commands;

public interface IBotCommand
{
    string Name { get; }
    string CallbackQueryPattern { get; }
    Task Execute(Message message, ITelegramBotClient client, IUnitOfWork services);
    Task Execute(CallbackQuery callbackQuery, ITelegramBotClient client, IUnitOfWork services);
    bool Contains(Message message);
    bool Contains(string callbackQueryData);
}
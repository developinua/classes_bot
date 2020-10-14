namespace TarasZoukClasses.Domain.Commands.Contract
{
    using System.Threading.Tasks;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public interface ICommand
    {
        string Name { get; }

        string CallbackQueryPattern { get; }

        Task Execute(Message message, TelegramBotClient client, IUnitOfWork services);

        Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services);

        bool Contains(Message message);

        bool Contains(string callbackQueryData);
    }
}

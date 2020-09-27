namespace TarasZoukClasses.Domain.Commands.CommandContract
{
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public interface ICommand
    {
        string Name { get; }

        string CallbackQueryPattern { get; }

        string ResponseMessage { get; set; }

        Task Execute(Message message, TelegramBotClient client);

        Task Execute(CallbackQuery callbackQuery, TelegramBotClient botClient);

        bool Contains(Message message);

        bool Contains(string callbackQueryData);
    }
}

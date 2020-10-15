namespace TarasZoukClasses.Domain.Commands.Administration.Admin
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class AdminCommand : ICommand
    {
        public string Name => "/admin";

        public string CallbackQueryPattern => "Not Implemented";

        public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

        public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

        public async Task Execute(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            var chatId = message.From.Id;

            if (AdministrationHelper.CanExecuteCommand(message.From.Username))
                await client.SendTextMessageAsync(chatId, "Access denied. You can't execute this command.");

            var responseMessage = $"/seed /paymentlink /managesubscriptions";
        }

        public Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services)
        {
            return Task.CompletedTask;
        }
    }
}

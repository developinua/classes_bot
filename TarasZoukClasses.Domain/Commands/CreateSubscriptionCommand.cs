namespace TarasZoukClasses.Domain.Commands
{
    using System.Threading.Tasks;
    using Contract;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public class CreateSubscriptionCommand : ICommand
    {
        public string Name => throw new System.NotImplementedException();

        public string CallbackQueryPattern => throw new System.NotImplementedException();

        public string ResponseMessage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public bool Contains(Message message)
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(string callbackQueryData)
        {
            throw new System.NotImplementedException();
        }

        public Task Execute(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            throw new System.NotImplementedException();
        }

        public Task Execute(CallbackQuery callbackQuery, TelegramBotClient botClient, IUnitOfWork services)
        {
            throw new System.NotImplementedException();
        }
    }
}

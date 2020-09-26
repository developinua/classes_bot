namespace TarasZoukClasses.Domain.Commands
{
    using System.Threading.Tasks;
    using CommandContract;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public class CreateSubscriptionCommand : ICommand
    {
        public string CommandName => throw new System.NotImplementedException();

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

        public Task Execute(Message message, TelegramBotClient client)
        {
            throw new System.NotImplementedException();
        }

        public Task Execute(CallbackQuery callbackQuery, TelegramBotClient botClient)
        {
            throw new System.NotImplementedException();
        }
    }
}

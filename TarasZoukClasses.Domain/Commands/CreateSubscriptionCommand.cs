namespace TarasZoukClasses.Domain.Commands
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class CreateSubscriptionCommand : ICommand
    {
        public string Name => @"/createSubscription";

        public string CallbackQueryPattern => @"(?i)(?<query>createSubscription)";

        private const string ResponseMessage = "";

        public bool Contains(Message message)
        {
            return message.Type == MessageType.Text && message.Text.Contains(Name);
        }

        public bool Contains(string callbackQueryData)
        {
            return new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;
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

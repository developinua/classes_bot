namespace TarasZoukClasses.Domain.Commands
{
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class CheckInCommand : ICommand
    {
        public string Name => @"/checkin";

        public string CallbackQueryPattern => @"(?i)(?<query>checkin)";

        private const string ResponseMessage = "Привіт! 😊\n\n" +
                                               "Якою мовою ти бажаєш спілкуватися?\n" +
                                               "What language do you want to communicate in?\n" +
                                               "На каком языке ты хочешь общаться?";

        private const string ResponseCallbackQueryMessage = "What do you want to do next?";

        public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

        public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

        public Task Execute(Message message, TelegramBotClient botClient, IUnitOfWork services)
        {
            return Task.CompletedTask;
        }

        public Task Execute(CallbackQuery callbackQuery, TelegramBotClient botClient, IUnitOfWork services)
        {
            return Task.CompletedTask;
        }
    }
}

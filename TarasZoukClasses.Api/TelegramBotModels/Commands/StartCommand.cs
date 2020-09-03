namespace TarasZoukClasses.Api.TelegramBotModels.Commands
{
    using System;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class StartCommand : ICommand
    {
        public string Name => @"/start";

        public bool Contains(Message message)
        {
            return message.Type == MessageType.Text && message.Text.Contains(Name);
        }

        public async Task Execute(Message message, TelegramBotClient botClient)
        {
            if (botClient == null)
            {
                throw new ArgumentNullException(nameof(TelegramBotClient));
            }

            await botClient.SendTextMessageAsync(message?.Chat?.Id,
                "Hello I'm ASP.NET Core Bot",
                parseMode: ParseMode.Markdown);
        }
    }
}

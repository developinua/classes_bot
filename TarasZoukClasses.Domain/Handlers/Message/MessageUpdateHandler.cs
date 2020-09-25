namespace TarasZoukClasses.Domain.Handlers.Message
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using TarasZoukClasses.Models.TelegramBotModels;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public class MessageUpdateHandler : IUpdateHandler
    {
        private TelegramBotClient TelegramBotClient { get; }

        public MessageUpdateHandler(TelegramBotClient telegramBotClient)
        {
            TelegramBotClient = telegramBotClient;
        }

        public async Task<UpdateHandlerResponse> Handle(Update update)
        {
            var message = update?.Message;

            if (message == null)
            {
                return new UpdateHandlerResponse
                {
                    ResponseType = UpdateHandlerResponseType.Error,
                    Message = $"Message update is null. {DateTime.UtcNow}."
                };
            }

            var commands = await TelegramBot.GetActiveCommandsAsync();

            foreach (var command in commands.Where(command => command.Contains(message)))
            {
                await command.Execute(message, TelegramBotClient);
                break;
            }

            return new UpdateHandlerResponse
            {
                ResponseType = UpdateHandlerResponseType.Ok
            };
        }
    }
}

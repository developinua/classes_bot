namespace TarasZoukClasses.Domain.Handlers.UpdateHandler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Service;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using UpdateHandlerContract;
    using UpdateHandlerResponse;

    public class CallbackQueryUpdateHandler : IUpdateHandler
    {
        private TelegramBotClient TelegramBotClient { get; }

        public CallbackQueryUpdateHandler(TelegramBotClient telegramBotClient)
        {
            TelegramBotClient = telegramBotClient;
        }

        public async Task<UpdateHandlerResponse> Handle(Update update)
        {
            var callbackQuery = update?.CallbackQuery;

            if (callbackQuery == null)
            {
                return new UpdateHandlerResponse
                {
                    ResponseType = UpdateHandlerResponseType.Error,
                    Message = $"Message update is null. {DateTime.UtcNow}."
                };
            }

            // TODO: Change it.
            var commands = await TelegramBot.GetActiveCommandsAsync();

            foreach (var command in commands.Where(command => command.Contains(callbackQuery.Data)))
            {
                await command.Execute(callbackQuery, TelegramBotClient);
                break;
            }

            return new UpdateHandlerResponse
            {
                ResponseType = UpdateHandlerResponseType.Ok
            };
        }
    }
}

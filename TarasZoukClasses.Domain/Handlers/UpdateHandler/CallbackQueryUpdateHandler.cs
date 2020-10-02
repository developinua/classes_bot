namespace TarasZoukClasses.Domain.Handlers.UpdateHandler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using UpdateHandlerContract;
    using UpdateHandlerResponse;

    public class CallbackQueryUpdateHandler : IUpdateHandler
    {
        private TelegramBotClient TelegramBotClient { get; }

        private IUnitOfWork Services { get; }

        public CallbackQueryUpdateHandler(TelegramBotClient telegramBotClient, IUnitOfWork services)
        {
            TelegramBotClient = telegramBotClient;
            Services = services;
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

            var commands = await Services.Commands.GetActiveCommandsAsync();

            foreach (var command in commands.Where(command => command.Contains(callbackQuery.Data)))
            {
                await command.Execute(callbackQuery, TelegramBotClient, Services);
                break;
            }

            return new UpdateHandlerResponse
            {
                ResponseType = UpdateHandlerResponseType.Ok
            };
        }
    }
}

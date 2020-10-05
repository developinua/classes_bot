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
            var userCommand = commands.SingleOrDefault(command => command.Contains(callbackQuery.Data));

            if (userCommand == null)
            {
                return new UpdateHandlerResponse
                {
                    Message = $"This bot can't process callback query: {callbackQuery.Data}.",
                    ResponseType = UpdateHandlerResponseType.Error
                };
            }

            await userCommand.Execute(callbackQuery, TelegramBotClient, Services);

            return new UpdateHandlerResponse
            {
                ResponseType = UpdateHandlerResponseType.Ok
            };
        }
    }
}

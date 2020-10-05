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

    public class MessageUpdateHandler : IUpdateHandler
    {
        private TelegramBotClient TelegramBotClient { get; }

        private IUnitOfWork Services { get; }

        public MessageUpdateHandler(TelegramBotClient telegramBotClient, IUnitOfWork services)
        {
            TelegramBotClient = telegramBotClient;
            Services = services;
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

            var commands = await Services.Commands.GetActiveCommandsAsync();
            var userCommand = commands.SingleOrDefault(command => command.Contains(message));

            if (userCommand == null)
            {
                return new UpdateHandlerResponse
                {
                    Message = $"Can't process message {message}",
                    ResponseType = UpdateHandlerResponseType.Error
                };
            }

            await userCommand.Execute(message, TelegramBotClient, Services);

            return new UpdateHandlerResponse
            {
                ResponseType = UpdateHandlerResponseType.Ok
            };
        }
    }
}

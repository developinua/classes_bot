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

            foreach (var command in commands.Where(command => command.Contains(message)))
            {
                await command.Execute(message, TelegramBotClient, Services);
                break;
            }

            return new UpdateHandlerResponse
            {
                ResponseType = UpdateHandlerResponseType.Ok
            };
        }
    }
}

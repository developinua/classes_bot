namespace TarasZoukClasses.Api.Handlers.UpdateHandler
{
    using System;
    using System.Threading.Tasks;
    using Domain.Commands.CheckIn;
    using Domain.Service.BaseService;
    using Domain.Utils;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using UpdateHandlerContract;
    using UpdateHandlerResponse;

    public class LocationUpdateHandler : IUpdateHandler
    {
        private TelegramBotClient TelegramBotClient { get; }

        private IUnitOfWork Services { get; }

        public LocationUpdateHandler(TelegramBotClient telegramBotClient, IUnitOfWork services)
        {
            TelegramBotClient = telegramBotClient;
            Services = services;
        }

        public async Task<UpdateHandlerResponse> Handle(Update update)
        {
            var isLocationDataValid = update.Message.ValidateMessageLocationData();

            if (!isLocationDataValid)
            {
                return new UpdateHandlerResponse
                {
                    ResponseType = UpdateHandlerResponseType.Error,
                    Message = $"Message update is null. {DateTime.UtcNow}."
                };
            }

            await new CheckInCommand().Execute(update.Message, TelegramBotClient, Services);

            return new UpdateHandlerResponse
            {
                ResponseType = UpdateHandlerResponseType.Ok
            };
        }
    }
}

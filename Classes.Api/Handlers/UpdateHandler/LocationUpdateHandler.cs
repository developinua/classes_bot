using System;
using System.Threading.Tasks;
using Classes.Domain.Commands.CheckIn;
using Classes.Domain.Repositories;
using Classes.Domain.Validators;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Api.Handlers.UpdateHandler;

public class LocationUpdateHandler : IUpdateHandler
{
    private ITelegramBotClient TelegramBotClient { get; }

    private IUnitOfWork Services { get; }

    public LocationUpdateHandler(ITelegramBotClient telegramBotClient, IUnitOfWork services)
    {
        TelegramBotClient = telegramBotClient;
        Services = services;
    }

    public async Task<UpdateHandlerResponse> Handle(Update update)
    {
        var isLocationDataValid = update.Message!.ValidateMessageLocationData();

        if (!isLocationDataValid)
        {
            return new UpdateHandlerResponse
            {
                ResponseType = UpdateHandlerResponseType.Error,
                Message = $"Message update is null. {DateTime.UtcNow}."
            };
        }

        await new CheckInCommand().Execute(update.Message!, TelegramBotClient, Services);

        return new UpdateHandlerResponse
        {
            ResponseType = UpdateHandlerResponseType.Ok
        };
    }
}
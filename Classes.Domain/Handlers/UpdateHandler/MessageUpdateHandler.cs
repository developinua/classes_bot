using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Classes.Domain.Repositories;
using Newtonsoft.Json;

namespace Classes.Domain.Handlers.UpdateHandler;

public class MessageUpdateHandler : IUpdateHandler
{
    private ITelegramBotClient TelegramBotClient { get; }

    private IUnitOfWork Services { get; }

    public MessageUpdateHandler(ITelegramBotClient telegramBotClient, IUnitOfWork services)
    {
        TelegramBotClient = telegramBotClient;
        Services = services;
    }

    public async Task<UpdateHandlerResponse> Handle(Update update)
    {
        var message = update.Message;

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
                Message = $"Can't process message {JsonConvert.SerializeObject(message)}",
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
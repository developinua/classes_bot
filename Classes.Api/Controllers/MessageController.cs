using System;
using System.Threading;
using System.Threading.Tasks;
using Classes.Domain.Extensions;
using Classes.Domain.Handlers.UpdateHandler;
using Classes.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Api.Controllers;

[Route("api/v1/[controller]")]
public class MessageController : Controller
{
    private readonly IUnitOfWork _db;
    private readonly ILogger<MessageController> _logger;
    private readonly ITelegramBotClient _botClient;

    public MessageController(
        ITelegramBotClient telegramBotClient, 
        IUnitOfWork services, 
        ILogger<MessageController> logger)
    {
        _botClient = telegramBotClient;
        _db = services;
        _logger = logger;
    }

    [HttpPost("update")]
    public async Task<IResult> MessageUpdate([FromBody] Update update, CancellationToken cancellationToken)
    {
        var handler = update.GetHandler(_botClient, _db);

        if (handler is null)
        {
            _logger.LogError("Update handler not found\nUpdate type: {updateType}\nUpdate message type: {messageType}",
                update.Type, update.Message?.Type.ToString() ?? "No message type was specified");
            return Results.BadRequest();
        }
	
        var handlerResponse = await handler.Handle(update);
        var chatId = update.Message?.Chat.Id ?? update.CallbackQuery!.From.Id;

        if (handlerResponse.ResponseType.IsError())
        {
            _logger.LogError("Chat id: {chatId}\nMessage:\n{errorMessage}", chatId.ToString(), handlerResponse.Message);
            await _botClient.SendTextMessageAsync(
                chatId, "Can't process message", ParseMode.Markdown, cancellationToken: cancellationToken);
            return Results.BadRequest();
        }

        _logger.LogInformation("Successful response from chat {chatId}. Date: {dateTime}", 
            chatId.ToString(), DateTime.UtcNow);

        return Results.Ok();
    }
}
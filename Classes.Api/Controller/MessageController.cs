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

namespace Classes.Api.Controller;

[Route("api/v1/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IUnitOfWork _db;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<MessageController> _logger;

    public MessageController(
        IUnitOfWork db,
        ITelegramBotClient botClient,
        ILogger<MessageController> logger)
    {
        _db = db;
        _botClient = botClient;
        _logger = logger;
    }
    
    [HttpPost("update")]
    public async Task<IResult> Update([FromBody] Update update, CancellationToken cancellationToken)
    {
        var updateHandler = update.GetHandler(_botClient, _db);

        if (updateHandler is null)
        {
            _logger.LogError(
                "Update handler not found\n" +
                "Update type: {updateType}\n" +
                "Update message type: {messageType}",
                update.Type,
                update.Message?.Type.ToString() ?? "No message type was specified");
        
            return Results.BadRequest();
        }
	
        // todo: change response to Result type
        var handlerResponse = await updateHandler.Handle(update);
        var chatId = update.Message?.Chat.Id ?? update.CallbackQuery!.From.Id;

        if (handlerResponse.ResponseType.IsError())
        {
            _logger.LogError(
                "Chat id: {chatId}\nMessage:\n{errorMessage}",
                chatId.ToString(),
                handlerResponse.Message);
            await _botClient.SendTextMessageAsync(
                chatId,
                "Can't process message",
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        
            return Results.BadRequest();
        }

        _logger.LogInformation("Successful response from chat {chatId}. Date: {dateTime}", 
            chatId.ToString(),
            DateTime.UtcNow);

        return Results.Ok();
    }
}
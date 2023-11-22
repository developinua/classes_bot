using System;
using System.Threading;
using System.Threading.Tasks;
using Classes.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResultNet;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Api.Controller;

[Route("api/v1/[controller]")]
public class MessageController : ControllerBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateService _updateService;
    private readonly ILogger<MessageController> _logger;

    public MessageController(
        ITelegramBotClient botClient,
        IUpdateService updateService,
        ILogger<MessageController> logger) =>
        (_botClient, _updateService, _logger) = (botClient, updateService, logger);

    [HttpPost("update")]
    public async Task<IResult> Update([FromBody] Update update, CancellationToken cancellationToken)
    {
        var handler = _updateService.GetHandler(update);

        if (handler is null)
        {
            _logger.LogError(
                "Update handler not found\n" +
                "Update type: {updateType}\n" +
                "Update message type: {messageType}",
                update.Type,
                update.Message?.Type.ToString() ?? "No message type was specified");
        
            return Results.BadRequest();
        }
	
        var response = await handler.Handle(update);
        // todo: check ids
        var chatId = update.Message?.From?.Id ?? update.CallbackQuery!.From.Id;

        if (response.IsFailure())
        {
            _logger.LogError(
                "Chat id: {chatId}\nMessage:\n{errorMessage}",
                chatId.ToString(),
                response.Message);
            
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
    
    // var affectedRows = await context.Person.Where(x => ids.Contains(x.Id))
    //     .ExecuteUpdateAsync(updates => updates.SetProperty(p => p.IsActive, false));
    //
    // return affectedRows == 0 ? Results.NotFound() : Results.NoContent();
}
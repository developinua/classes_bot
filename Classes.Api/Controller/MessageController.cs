using System;
using System.Threading;
using System.Threading.Tasks;
using Classes.App.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Api.Controller;

[Route("api/v1/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IBotService _botService;
    private readonly IUpdateService _updateService;
    private readonly IMediator _mediator;
    private readonly ILogger<MessageController> _logger;

    public MessageController(
        IBotService botService,
        IUpdateService updateService,
        IMediator mediator,
        ILogger<MessageController> logger)
    {
        _botService = botService;
        _updateService = updateService;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("update")]
    public async Task<IResult> Update([FromBody] Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message?.From?.Id ?? update.CallbackQuery!.From.Id;
        var request = _updateService.GetRequestFromUpdate(update);
        
        if (request.IsFailure())
            return await HandleFailureResponse(chatId, cancellationToken);
	
        var response = await _mediator.Send(request.Data, cancellationToken);

        if (response.IsFailure())
            return await HandleFailureResponse(chatId, cancellationToken, response);

        _logger.LogInformation(
            "Successful response from chat {chatId}. Date: {dateTime}", 
            chatId.ToString(),
            DateTime.UtcNow);

        return Results.Ok();
    }

    private async Task<IResult> HandleFailureResponse(
        long chatId,
        CancellationToken cancellationToken,
        Result? response = null)
    {
        _logger.LogError(
            "Chat id: {chatId}\nMessage:\n{errorMessage}",
            chatId.ToString(),
            response?.Message ?? "No message was specified");
            
        await _botService.SendTextMessageAsync(
            chatId,
            "Can't process message",
            cancellationToken: cancellationToken);
        
        return Results.BadRequest();
    }
}
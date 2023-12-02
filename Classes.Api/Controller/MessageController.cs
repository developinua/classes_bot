using System.Threading;
using System.Threading.Tasks;
using Classes.App.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Api.Controller;

[Route("api/v1/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IUpdateService _updateService;
    private readonly IMediator _mediator;

    public MessageController(IUpdateService updateService, IMediator mediator) =>
        (_updateService, _mediator) = (updateService, mediator);

    [HttpPost("update")]
    public async Task<IResult> Update([FromBody] Update update, CancellationToken cancellationToken)
    {
        var chatId = _updateService.GetChatId(update);
        var request = _updateService.GetRequestFromUpdate(update);

        if (request.IsFailure())
        {
            await _updateService.HandleFailureResponse(chatId, cancellationToken);
            return Results.BadRequest();
        }
	
        var response = await _mediator.Send(request.Data, cancellationToken);

        if (response.IsFailure())
        {
            await _updateService.HandleFailureResponse(chatId, cancellationToken, response.Message);
            return Results.BadRequest();
        }

        await _updateService.HandleSuccessResponse(chatId);

        return Results.Ok();
    }
}
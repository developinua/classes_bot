using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Api.Controller;

[Route("api/v1/[controller]")]
public class MessagesController(IUpdateService updateService, IMediator mediator) : ControllerBase
{
    [HttpPost("update")]
    public async Task<IResult> Update([FromBody] Update update, CancellationToken cancellationToken)
    {
        var chatId = updateService.GetChatId(update);
        var request = updateService.GetRequestFromUpdate(update);

        if (request.IsFailure())
        {
            await updateService.HandleFailureResponse(chatId, cancellationToken);
            return Results.Ok();
        }
	
        var response = await mediator.Send(request.Data, cancellationToken);

        if (response.IsFailure())
        {
            await updateService.HandleFailureResponse(chatId, cancellationToken, response.Message);
            return Results.Ok();
        }

        await updateService.HandleSuccessResponse(chatId);
        return Results.Ok();
    }
}
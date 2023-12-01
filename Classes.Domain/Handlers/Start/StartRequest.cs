using Classes.Domain.Models.BotRequest;
using MediatR;
using ResultNet;

namespace Classes.Domain.Handlers.Start;

public class StartRequest : BotMessageRequest, IRequest<Result>
{
    protected override string Name => "/start";
    public long ChatId { get; set; }
    public string? Username { get; set; }
}
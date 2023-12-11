using Classes.Domain.Requests.Bot;
using MediatR;
using ResultNet;

namespace Classes.Domain.Requests;

public class StartRequest : BotMessageRequest, IRequest<Result>
{
    protected override string Name => "/start";
    public long ChatId { get; set; }
    public string? Username { get; set; }
}
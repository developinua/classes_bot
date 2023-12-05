using Classes.Domain.BotRequest;
using MediatR;
using ResultNet;

namespace Classes.Domain.Requests;

public class SeedRequest : BotMessageRequest, IRequest<Result>
{
    protected override string Name => "/seed";
    public long ChatId { get; set; }
    public string Username { get; set; } = null!;
}
using Classes.Domain.Models.BotRequest;
using MediatR;
using ResultNet;

namespace Classes.Domain.Handlers.Administration.Seed;

public class SeedRequest : BotMessageRequest, IRequest<Result>
{
    protected override string Name => "/seed";
    public long ChatId { get; set; }
    public string Username { get; set; } = null!;
}
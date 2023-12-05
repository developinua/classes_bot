using Classes.Domain.BotRequest;
using MediatR;
using ResultNet;

namespace Classes.Domain.Requests;

public class SubscriptionsRequest : BotMessageRequest, IRequest<Result>
{
    protected override string Name => "/subscriptions";
    public long ChatId { get; set; }
    public string Username { get; set; } = null!;
}
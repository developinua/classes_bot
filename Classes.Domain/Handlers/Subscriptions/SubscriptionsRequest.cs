using Classes.Domain.Models.BotRequest;
using MediatR;
using ResultNet;

namespace Classes.Domain.Handlers.Subscriptions;

public class SubscriptionsRequest : BotMessageRequest, IRequest<Result>
{
    protected override string Name => "/subscriptions";
    public long ChatId { get; set; } //message.From!.Id
    public string Username { get; set; } = null!; //message.From.Username!
}
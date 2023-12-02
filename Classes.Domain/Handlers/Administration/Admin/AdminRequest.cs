using Classes.Domain.Models.BotRequest;
using MediatR;
using ResultNet;

namespace Classes.Domain.Handlers.Administration.Admin;

public class AdminRequest : BotMessageRequest, IRequest<Result>
{
    protected override string Name => "/admin";
    public long ChatId { get; set; }
    public string Username { get; set; } = null!;
}
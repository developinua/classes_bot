using Classes.Domain.Models.BotRequest;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Requests;

public class StartCallbackRequest : BotCallbackRequest, IRequest<Result>
{
    public override string CallbackPattern => @"(?i)(?<query>language):(?<data>\w{2}-\w{2})";
    public long ChatId { get; set; }
    public CallbackQuery CallbackQuery { get; set; } = null!;
}
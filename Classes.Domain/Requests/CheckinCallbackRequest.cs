using Classes.Domain.BotRequest;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Requests;

public class CheckinCallbackRequest : BotCallbackRequest, IRequest<Result>
{
    public override string CallbackPattern => "(?i)(?<query>check-in-subscription-id):(?<data>.*)";
    public long ChatId { get; set; }
    public CallbackQuery CallbackQuery { get; set; } = null!;
}
using Classes.Domain.Models.BotRequest;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers.Checkin;

public class CheckinCallbackRequest : BotCallbackRequest, IRequest<Result>
{
    public override string CallbackPattern => "(?i)(?<query>check-in-subscription-id):(?<data>.*)";
    public long ChatId { get; set; } //callbackQuery.From.Id
    public CallbackQuery CallbackQuery { get; set; } = null!;
}
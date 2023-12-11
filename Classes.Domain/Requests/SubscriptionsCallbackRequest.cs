using Classes.Domain.Requests.Bot;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Requests;

public class SubscriptionsCallbackRequest : BotCallbackRequest, IRequest<Result>
{
    public override string CallbackPattern => "(?i)(?<query>subsGroup|subsPeriod)";
    public long ChatId { get; set; }
    public CallbackQuery CallbackQuery { get; set; } = null!;
}
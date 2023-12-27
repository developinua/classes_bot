using Classes.Domain.Requests.Bot;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Requests;

public class SubscriptionsInformationCallbackRequest : BotCallbackRequest, IRequest<Result>
{
    public override string CallbackPattern => "(?i)(?<query>subscription):(?<data>class|course)";
    public long ChatId { get; set; }
    public string Username { get; set; } = null!;
    public CallbackQuery CallbackQuery { get; set; } = null!;
}
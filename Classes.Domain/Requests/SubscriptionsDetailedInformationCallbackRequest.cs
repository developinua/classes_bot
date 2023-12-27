using Classes.Domain.Requests.Bot;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Requests;

public class SubscriptionsDetailedInformationCallbackRequest : BotCallbackRequest, IRequest<Result>
{
    public override string CallbackPattern => @"(?i)(?<query>user-subscription-id):(?<data>\d+)";
    public long ChatId { get; set; }
    public string Username { get; set; } = null!;
    public CallbackQuery CallbackQuery { get; set; } = null!;
}
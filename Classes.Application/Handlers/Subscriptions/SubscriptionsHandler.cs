using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;

namespace Classes.Application.Handlers.Subscriptions;

public class SubscriptionsHandler(
        IBotService botService,
        IReplyMarkupService replyMarkupService,
        IUserSubscriptionService userSubscriptionService,
        IStringLocalizer<SubscriptionsHandler> localizer)
    : IRequestHandler<SubscriptionsRequest, Result>
{
    public async Task<Result> Handle(SubscriptionsRequest request, CancellationToken cancellationToken)
    {
        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);

        var userSubscriptions = await userSubscriptionService.GetUserSubscriptions(request.Username);

        if (userSubscriptions.Data.Count == 0)
        {
            await botService.SendTextMessageAsync(localizer.GetString("NoSubscriptions"), cancellationToken);
            return Result.Success();
        }

        await botService.SendTextMessageWithReplyAsync(
            localizer.GetString("ChooseSubscriptionType"),
            replyMarkup: replyMarkupService.GetSubscriptions(),
            cancellationToken);

        return Result.Success();
    }
}
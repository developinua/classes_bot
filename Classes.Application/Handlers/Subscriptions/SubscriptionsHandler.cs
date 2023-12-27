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

        if (userSubscriptions.Data.Count != 0)
        {
            await botService.SendTextMessageAsync(localizer.GetString("NoSubscriptions"),cancellationToken);
            return Result.Success();
        }

        await botService.SendTextMessageWithReplyAsync(
            localizer.GetString("ChooseSubscriptionType"),
            replyMarkup: replyMarkupService.GetSubscriptions(),
            cancellationToken);

        return Result.Success();
    }
    
    // private async Task SendUserSubscriptions(
    //     long chatId,
    //     IReadOnlyCollection<UserSubscription> userSubscriptions,
    //     CancellationToken cancellationToken)
    // {
    //     var message = userSubscriptions.Count > 1 
    //         ? localizer.GetString("YourSubscriptions")
    //         : localizer.GetString("YourSubscription");
    //
    //     botService.UseChat(chatId);
    //     await botService.SendTextMessageAsync(message, cancellationToken);
    //
    //     foreach (var userSubscription in userSubscriptions)
    //     {
    //         // todo: localize
    //         var replyMessage =
    //             $"{localizer["Subscription", localizer[userSubscription.Subscription.NameCode]]}\n" +
    //             $"{localizer["Description", localizer[userSubscription.Subscription.DescriptionCode]]}\n" +
    //             $"{localizer["SubscriptionType", localizer[userSubscription.Subscription.Type.DisplayName()]]}\n" +
    //             $"{localizer["RemainingClasses", localizer[userSubscription.RemainingClasses.ToString()]]}\n";
    //         await botService.SendTextMessageAsync(replyMessage, cancellationToken);
    //     }
    //
    //     // TODO: add functionality for adding multiple subscriptions
    //     await botService.SendTextMessageAsync(localizer.GetString("DoYouWantToCheckin"), cancellationToken);
    // }
}
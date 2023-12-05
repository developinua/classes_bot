using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Checkin;

public class CheckinCallbackHandler(
        IBotService botService,
        ICallbackExtractorService callbackExtractorService,
        IUserSubscriptionService userSubscriptionService,
        IValidator<CallbackQuery> callbackQueryValidator)
    : IRequestHandler<CheckinCallbackRequest, Result>
{
    public async Task<Result> Handle(CheckinCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await callbackQueryValidator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            return Result.Failure().WithMessage("Invalid check-in parameters.");

        await botService.SendChatActionAsync(request.ChatId, cancellationToken);

        var userSubscriptionId = callbackExtractorService.GetUserSubscriptionIdFromCallback(
            request.CallbackQuery.Data!,
            request.CallbackPattern);
        var userSubscription = await userSubscriptionService.GetById(userSubscriptionId);

        if (userSubscription.Data is null)
        {
            await botService.SendTextMessageAsync(
                request.ChatId,
                "You haven't that subscription. Please contact @nazikBro for details.",
                cancellationToken);
            return Result.Failure().WithMessage("User subscription not found.");
        }

        if (userSubscription.Data.RemainingClasses == 0)
        {
            await botService.SendTextMessageAsync(
                request.ChatId,
                "You haven't any available classes. Press /subscriptions to manage your subscriptions.",
                cancellationToken);
            return Result.Failure().WithMessage("No remaining classes left.");
        }
        
        var checkinResult = await userSubscriptionService.CheckinOnClass(userSubscription.Data);

        if (checkinResult.IsFailure())
        {
            await botService.SendTextMessageAsync(
                request.ChatId,
                "There was a problem with class check-in. Please contact @nazikBro for details.",
                cancellationToken);
            return Result.Failure().WithMessage("There was a problem with class check-in.");
        }
        
        await botService.SendTextMessageAsync(request.ChatId, "*💚*", cancellationToken);
        return Result.Success();
    }
}
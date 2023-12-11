using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Checkin;

public class CheckinCallbackHandler(
        IBotService botService,
        IUserSubscriptionService userSubscriptionService,
        ICallbackExtractorService callbackExtractorService,
        IStringLocalizer<CheckinHandler> localizer,
        IValidator<CallbackQuery> callbackQueryValidator)
    : IRequestHandler<CheckinCallbackRequest, Result>
{
    public async Task<Result> Handle(CheckinCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await callbackQueryValidator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            return Result.Failure().WithMessage("Invalid check-in parameters.");

        botService.UseChat(request.ChatId);

        await botService.SendChatActionAsync(cancellationToken);

        var userSubscriptionId = callbackExtractorService.GetUserSubscriptionIdFromCallback(
            request.CallbackQuery.Data!, request.CallbackPattern);
        var userSubscription = await userSubscriptionService.GetById(userSubscriptionId);

        if (userSubscription.Data is null)
        {
            await botService.SendTextMessageAsync(localizer.GetString("SubscriptionNotFound"), cancellationToken);
            return Result.Failure().WithMessage("User subscription not found.");
        }

        if (!userSubscriptionService.CanCheckinOnClass(userSubscription.Data))
        {
            await botService.SendTextMessageAsync(localizer.GetString("NoAvailableClasses"), cancellationToken);
            return Result.Failure().WithMessage("No available classes.");
        }
        
        var checkinResult = await userSubscriptionService.CheckinOnClass(userSubscription.Data);

        if (checkinResult.IsFailure())
        {
            await botService.SendTextMessageAsync(localizer.GetString("ClassCheckinProblem"), cancellationToken);
            return Result.Failure().WithMessage("There was a problem with class check-in.");
        }
        
        await botService.SendTextMessageAsync("*💚*", cancellationToken);

        return Result.Success();
    }
}
using System.Linq;
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
        IUserSubscriptionService userSubscriptionService,
        IValidator<CallbackQuery> callbackQueryValidator)
    : IRequestHandler<CheckinCallbackRequest, Result>
{
    public async Task<Result> Handle(CheckinCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await callbackQueryValidator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            return Result.Failure().WithMessage("Invalid check-in parameters.");

        await botService.SendChatActionAsync(request.ChatId, cancellationToken);

        var userSubscription = await userSubscriptionService.GetFromCallback(
            request.CallbackQuery, request.CallbackPattern);

        if (userSubscription.Data is null)
            return await HandleSubscriptionNotFoundFailure();
        
        var checkinResult = await userSubscriptionService.CheckinOnClass(userSubscription.Data);

        if (checkinResult.IsFailure())
            return await HandleCheckinFailure();
        
        await botService.SendTextMessageAsync(request.ChatId, "*💚*", cancellationToken);
        return Result.Success();
        
        async Task<Result> HandleSubscriptionNotFoundFailure()
        {
            await botService.SendTextMessageAsync(
                request.ChatId,
                "You haven't this subscription. Please contact @nazikBro for details.",
                cancellationToken);
            return Result.Failure().WithMessage("User subscription not found.");
        }
        
        async Task<Result> HandleCheckinFailure()
        {
            if (checkinResult.Errors.Any())
            {
                await botService.SendTextMessageAsync(
                    request.ChatId,
                    checkinResult.Errors.First().Message,
                    cancellationToken);
                return Result.Failure().WithMessage(checkinResult.Errors.First().Message);
            }

            await botService.SendTextMessageAsync(
                request.ChatId,
                "There was a problem with class check-in. Please contact @nazikBro for details.",
                cancellationToken);
            return Result.Failure().WithMessage("There was a problem with class check-in.");
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Start;

public class StartCallbackHandler(
        IBotService botService,
        IUserService userService,
        ICultureService cultureService,
        ICallbackExtractorService callbackExtractorService,
        IValidator<CallbackQuery> validator)
    : IRequestHandler<StartCallbackRequest, Result>
{
    public async Task<Result> Handle(StartCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            return Result.Failure().WithMessage("Invalid callback query.");

        await botService.SendChatActionAsync(request.ChatId, cancellationToken);
        
        var cultureName = callbackExtractorService.GetCultureNameFromCallbackQuery(
            request.CallbackQuery.Data, request.CallbackPattern);
        var culture = await cultureService.GetByName(cultureName);
        var result = await userService.SaveUser(request, culture);
        
        if (result.IsFailure()) return result;
        
        await botService.SendTextMessageAsync(
            request.ChatId,
            "*😊Successfully!😊*\nPress /subscriptions to manage your class subscription.",
            cancellationToken);
        
        return Result.Success();
    }
}
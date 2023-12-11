using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Start;

public class StartCallbackHandler(
        IBotService botService,
        IUserService userService,
        ICultureService cultureService,
        ICallbackExtractorService callbackExtractorService,
        IStringLocalizer<StartHandler> localizer,
        IValidator<CallbackQuery> validator)
    : IRequestHandler<StartCallbackRequest, Result>
{
    public async Task<Result> Handle(StartCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            return Result.Failure().WithMessage("Invalid callback query.");

        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);
        
        var cultureName = callbackExtractorService.GetCultureNameFromCallbackQuery(
            request.CallbackQuery.Data, request.CallbackPattern);
        var culture = await cultureService.GetByName(cultureName);
        var result = await userService.SaveUser(request, culture);
        
        if (result.IsFailure()) return result;
        
        await botService.SendTextMessageAsync(localizer.GetString("ManageClassSubscriptions"), cancellationToken);
        
        return Result.Success();
    }
}
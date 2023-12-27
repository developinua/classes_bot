using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Language;

public class LanguageCallbackHandler(
        IBotService botService,
        IUserProfileService userProfileService,
        ICultureService cultureService,
        ICallbackExtractorService callbackExtractorService,
        IStringLocalizer<LanguageHandler> localizer,
        IValidator<CallbackQuery> validator)
    : IRequestHandler<LanguageCallbackRequest, Result>
{
    public async Task<Result> Handle(LanguageCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            return Result.Failure().WithMessage("Invalid callback query.");

        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);
        
        var cultureName = callbackExtractorService.GetCultureNameFrom(
            request.CallbackQuery.Data,
            request.CallbackPattern);
        var culture = await cultureService.GetByName(cultureName);
        var userProfile = userProfileService.GetUserProfileFromCallback(request.CallbackQuery, culture);
        var updateResult = await userProfileService.UpdateUserProfile(userProfile);
        
        if (updateResult.IsFailure()) return updateResult;
        
        await botService.SendTextMessageAsync(localizer.GetString("ManageClasses"), cancellationToken);
        return Result.Success();
    }
}
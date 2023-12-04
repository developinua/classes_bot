using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Handlers.Start;

public class StartCallbackHandler : IRequestHandler<StartCallbackRequest, Result>
{
    private readonly IBotService _botService;
    private readonly IUserService _userService;
    private readonly ICultureService _cultureService;
    private readonly IValidator<CallbackQuery> _validator;
    private readonly ICallbackExtractorService _callbackExtractorService;

    public StartCallbackHandler(
        IBotService botService,
        IUserService userService,
        ICultureService cultureService,
        ICallbackExtractorService callbackExtractorService,
        IValidator<CallbackQuery> validator)
    {
        _botService = botService;
        _userService = userService;
        _cultureService = cultureService;
        _callbackExtractorService = callbackExtractorService;
        _validator = validator;
    }

    public async Task<Result> Handle(StartCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await _validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            return Result.Failure().WithMessage("Invalid callback query.");

        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);
        
        var cultureName = _callbackExtractorService.GetCultureNameFromCallbackQuery(
            request.CallbackQuery.Data, request.CallbackPattern);
        var culture = await _cultureService.GetByName(cultureName);
        var saveUserResult = await _userService.SaveUser(request, culture);
        
        if (saveUserResult.IsFailure())
            return saveUserResult;
        
        await _botService.SendTextMessageAsync(
            request.ChatId,
            "*😊Successfully!😊*\nPress /subscriptions to manage your class subscription.",
            cancellationToken);
        
        return Result.Success();
    }
}
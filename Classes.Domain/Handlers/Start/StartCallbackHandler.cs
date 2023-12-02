using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Classes.Domain.Services;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers.Start;

public class StartCallbackHandler : IRequestHandler<StartCallbackRequest, Result>
{
    private readonly IBotService _botService;
    private readonly IUserService _userService;
    private readonly IValidator<CallbackQuery> _validator;

    public StartCallbackHandler(
        IUserService userService,
        IBotService botService,
        IValidator<CallbackQuery> validator)
    {
        _userService = userService;
        _botService = botService;
        _validator = validator;
    }

    public async Task<Result> Handle(StartCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await _validator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            throw new NotSupportedException();

        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);
        
        var cultureName = GetCultureNameFromCallbackQuery(request.CallbackQuery.Data, request.CallbackPattern);
        await _userService.SaveUser(request.CallbackQuery, cultureName);
        await _botService.SendTextMessageAsync(
            request.ChatId,
            "*😊Successfully!😊*\nPress /subscriptions to manage your class subscription.",
            cancellationToken);
        
        return Result.Success();
    }
    
    private static string GetCultureNameFromCallbackQuery(string? callbackData, string callbackPattern)
    {
        var cultureName = string.Empty;
        var cultureMatch = Regex.Match(callbackData ?? "", callbackPattern);

        if (cultureMatch.Success && cultureMatch.Groups["query"].Value.Equals("language"))
            cultureName = cultureMatch.Groups["data"].Value;

        if (string.IsNullOrEmpty(cultureName))
            throw new ArgumentException("Culture name can't be parsed.");

        return cultureName;
    }
}
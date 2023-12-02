using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Classes.Domain.Services;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers.Checkin;

public class CheckinCallbackHandler : IRequestHandler<CheckinCallbackRequest, Result>
{
    private readonly IBotService _botService;
    private readonly IUserSubscriptionService _userSubscriptionService;
    private readonly IValidator<CallbackQuery> _callbackQueryValidator;

    public CheckinCallbackHandler(
        IBotService botService,
        IUserSubscriptionService userSubscriptionService,
        IValidator<CallbackQuery> callbackQueryValidator)
    {
        _botService = botService;
        _userSubscriptionService = userSubscriptionService;
        _callbackQueryValidator = callbackQueryValidator;
    }

    public async Task<Result> Handle(CheckinCallbackRequest request, CancellationToken cancellationToken)
    {
        if ((await _callbackQueryValidator.ValidateAsync(request.CallbackQuery, cancellationToken)).IsValid)
            throw new NotSupportedException();

        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);

        var userSubscriptionId = GetUserSubscriptionIdFromCallbackQuery(
            request.CallbackQuery.Data!,
            request.CallbackPattern);
        var userSubscription = await _userSubscriptionService.GetById(userSubscriptionId);

        if (userSubscription.Data is null)
        {
            await _botService.SendTextMessageAsync(
                request.ChatId,
                "Can't get user subscription from db. Please contact @nazikBro for details",
                cancellationToken);
            return Result.Failure();
        }

        if (userSubscription.Data.RemainingClasses == 0)
        {
            await _botService.SendTextMessageAsync(
                request.ChatId,
                "You haven't any available classes. Press /subscriptions to manage your subscriptions",
                cancellationToken);
            return Result.Failure();
        }

        userSubscription.Data.RemainingClasses--;
        await _userSubscriptionService.Update(userSubscription.Data);
        
        await _botService.SendTextMessageAsync(
            request.ChatId,
            "*💚*",
            cancellationToken);

        return Result.Success();
    }
    
    private static long GetUserSubscriptionIdFromCallbackQuery(string callbackQueryData, string callbackQueryPattern)
    {
        var userSubscriptionIdGroup = string.Empty;
        var userSubscriptionIdGroupMatch = Regex.Match(callbackQueryData, callbackQueryPattern);
        var userSubscriptionIdGroupMatchQuery = userSubscriptionIdGroupMatch.Groups["query"].Value;
        var userSubscriptionIdGroupMatchData = userSubscriptionIdGroupMatch.Groups["data"].Value;

        if (userSubscriptionIdGroupMatch.Success
            && userSubscriptionIdGroupMatchQuery.Equals("checkinUserSubscriptionId"))
            userSubscriptionIdGroup = userSubscriptionIdGroupMatchData;

        return long.Parse(userSubscriptionIdGroup);
    }
}
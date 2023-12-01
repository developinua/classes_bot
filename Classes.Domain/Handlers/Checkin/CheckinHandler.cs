using System.Threading;
using System.Threading.Tasks;
using Classes.Domain.Services;
using FluentValidation;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Handlers.Checkin;

public class CheckinHandler : IRequestHandler<CheckinRequest, Result>
{
    private readonly IBotService _botService;
    private readonly IUserSubscriptionService _userSubscriptionService;
    private readonly IValidator<Message> _locationValidator;

    public CheckinHandler(
        IBotService botService,
        IUserSubscriptionService userSubscriptionService,
        IValidator<Message> locationValidator)
    {
        _botService = botService;
        _userSubscriptionService = userSubscriptionService;
        _locationValidator = locationValidator;
    }

    public async Task<Result> Handle(CheckinRequest request, CancellationToken cancellationToken)
    {
        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);

        var isLocationDataValid = (await _locationValidator.ValidateAsync(request.Message, cancellationToken)).IsValid;

        if (isLocationDataValid)
        {
            await _userSubscriptionService.ShowUserSubscriptionsInformation(
                request.ChatId, request.Username, cancellationToken);
            return Result.Failure();
        }

        var replyMarkup = new ReplyKeyboardMarkup(KeyboardButton.WithRequestLocation("Send location"))
        {
            OneTimeKeyboard = true,
            ResizeKeyboard = true
        };

        await _botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            "Please send me your location, so I can check if you are on classes now.",
            replyMarkup,
            cancellationToken);
        
        return Result.Success();
    }
}
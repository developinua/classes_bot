using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Application.Handlers.Start;

public class StartHandler : IRequestHandler<StartRequest, Result>
{
    private readonly IBotService _botService;
    private readonly IReplyMarkupService _replyMarkupService;
    private readonly ILogger<StartHandler> _logger;
    
    public StartHandler(
        IBotService botService,
        IReplyMarkupService replyMarkupService,
        ILogger<StartHandler> logger)
    {
        _botService = botService;
        _replyMarkupService = replyMarkupService;
        _logger = logger;
    }

    public async Task<Result> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);

        if (string.IsNullOrEmpty(request.Username))
            return await HandleFailureResponse(request, cancellationToken);

        await _botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            "*😊 Hi!\n\n*What language do you want to communicate in?",
            _replyMarkupService.GetStartMarkup(),
            cancellationToken);
        
        return Result.Success();
    }

    private async Task<Result> HandleFailureResponse(StartRequest request, CancellationToken cancellationToken)
    {
        await _botService.SendTextMessageAsync(
            request.ChatId,
            "Fill in the username in your account settings and then press /start again",
            cancellationToken);
        _logger.LogWarning("Chat: {ChatId}. Username is null or empty.", request.ChatId);

        return Result.Failure().WithMessage("Username is null or empty.");
    }
}
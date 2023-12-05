using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Application.Handlers.Start;

public class StartHandler(
        IBotService botService,
        IReplyMarkupService replyMarkupService,
        ILogger<StartHandler> logger)
    : IRequestHandler<StartRequest, Result>
{
    public async Task<Result> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        await botService.SendChatActionAsync(request.ChatId, cancellationToken);

        if (string.IsNullOrEmpty(request.Username))
        {
            await botService.SendTextMessageAsync(
                request.ChatId,
                "Fill in the username in your account settings and then press /start again",
                cancellationToken);
            logger.LogWarning("Chat: {ChatId}. Username is null or empty.", request.ChatId);

            return Result.Failure().WithMessage("Username is null or empty.");
        }

        await botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            "*😊 Hi!\n\n*What language do you want to communicate in?",
            replyMarkupService.GetStartMarkup(),
            cancellationToken);
        
        return Result.Success();
    }
}
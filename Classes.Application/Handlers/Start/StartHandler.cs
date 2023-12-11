using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;

namespace Classes.Application.Handlers.Start;

public class StartHandler(
        IBotService botService,
        IReplyMarkupService replyMarkupService,
        IStringLocalizer<StartHandler> localizer)
    : IRequestHandler<StartRequest, Result>
{
    public async Task<Result> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);

        if (string.IsNullOrEmpty(request.Username))
        {
            await botService.SendTextMessageAsync(localizer.GetString("UsernameIsNotFilledIn"), cancellationToken);
            return Result.Failure().WithMessage("Can't start the process. Username is null or empty.");
        }

        await botService.SendTextMessageWithReplyAsync(
            localizer.GetString("CommunicationLanguage"),
            replyMarkupService.GetStartMarkup(),
            cancellationToken);
        
        return Result.Success();
    }
}
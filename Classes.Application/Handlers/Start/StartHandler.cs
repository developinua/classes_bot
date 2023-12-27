using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Classes.Application.Services;
using Classes.Domain.Requests;
using MediatR;
using Microsoft.Extensions.Localization;
using ResultNet;

namespace Classes.Application.Handlers.Start;

public class StartHandler(
        IBotService botService,
        IUserService userService,
        IUpdateService updateService,
        ICultureService cultureService,
        IStringLocalizer<StartHandler> localizer,
        IMediator mediator,
        IMapper mapper)
    : IRequestHandler<StartRequest, Result>
{
    public async Task<Result> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        botService.UseChat(request.ChatId);
        await botService.SendChatActionAsync(cancellationToken);

        if (await userService.UserAlreadyRegistered(request.Username))
        {
            await botService.SendTextMessageAsync(localizer.GetString("BotAlreadyStarted"), cancellationToken);
            return Result.Success();
        }

        var botUserCultureName = updateService.GetUserCultureName(request.Message);
        var culture = await cultureService.GetByName(botUserCultureName);
        var result = await userService.SaveUser(request, culture);

        if (result.IsFailure()) return result;

        await mediator.Send(mapper.Map<LanguageRequest>(request), cancellationToken);
        return Result.Success();
    }
}
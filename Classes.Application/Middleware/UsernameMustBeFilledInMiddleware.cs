using System.Threading.Tasks;
using Classes.Application.Handlers.Start;
using Classes.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Classes.Application.Middleware;

public class UsernameMustBeFilledInMiddleware(
        IUpdateService updateService,
        IBotService botService,
        IStringLocalizer<StartHandler> localizer,
        ILogger<UsernameMustBeFilledInMiddleware> logger)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var update = await updateService.GetUpdateFromHttpRequest(context);
        var username = updateService.GetUsername(update);

        if (!string.IsNullOrEmpty(username))
        {
            await next(context);
            return;
        }

        var chatId = updateService.GetChatId(update);
        
        botService.UseChat(chatId);
        await botService.SendTextMessageAsync(localizer.GetString("UsernameIsNotFilledIn"), default);
        logger.LogWarning("Username is not filled in for chat {ChatId}", chatId);
        
        context.Response.StatusCode = 200;
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Classes.Domain.Extensions;
using Classes.Domain.Handlers.UpdateHandler;
using Classes.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAppServices(builder.Configuration);
services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());

await services.UseTelegramBotWebHooks(builder.Configuration);

var app = builder.Build();

app.MapPut("api/v1/message/update", async (
    [FromBody] Update update,
    IUnitOfWork db,
    ITelegramBotClient botClient,
    ILogger logger,
    CancellationToken cancellationToken) =>
{
    var updateHandler = update.GetHandler(botClient, db);

    if (updateHandler is null)
    {
        logger.LogError(
            "Update handler not found\n" +
            "Update type: {updateType}\n" +
            "Update message type: {messageType}",
            update.Type,
            update.Message?.Type.ToString() ?? "No message type was specified");
        return Results.BadRequest();
    }
	
    // todo: change response to Result type
    var handlerResponse = await updateHandler.Handle(update);
    var chatId = update.Message?.Chat.Id ?? update.CallbackQuery!.From.Id;

    if (handlerResponse.ResponseType.IsError())
    {
        logger.LogError(
            "Chat id: {chatId}\nMessage:\n{errorMessage}",
            chatId.ToString(),
            handlerResponse.Message);
        await botClient.SendTextMessageAsync(
            chatId,
            "Can't process message",
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
        return Results.BadRequest();
    }

    logger.LogInformation("Successful response from chat {chatId}. Date: {dateTime}", 
        chatId.ToString(),
        DateTime.UtcNow);

    return Results.Ok();
});

app.Run();
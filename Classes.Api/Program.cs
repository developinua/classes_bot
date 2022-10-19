using System;
using System.Threading;
using Classes.Api.Extensions;
using Classes.Api.Handlers.UpdateHandler;
using Classes.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepository();
await builder.Services.UseTelegramBotWebHooks(builder.Configuration);

var app = builder.Build();

app.MapPost("/update", async (
	[FromBody] Update update, 
	[FromServices] ITelegramBotClient botClient, 
	[FromServices] IUnitOfWork db, 
	[FromServices] ILogger logger,
	CancellationToken cancellationToken) =>
{
	var handler = update.GetHandler(botClient, db);

	if (handler is null)
	{
		logger.LogError("Update handler not found\nUpdate type: {updateType}\nUpdate message type: {messageType}",
			update.Type, update.Message?.Type.ToString() ?? "No message type was specified");
		return Results.BadRequest();
	}
	
	var handlerResponse = await handler.Handle(update);
	var chatId = update.Message?.Chat.Id ?? update.CallbackQuery!.From.Id;

	if (handlerResponse.ResponseType.IsError())
	{
		logger.LogError("Chat id: {chatId}\nMessage:\n{errorMessage}", chatId.ToString(), handlerResponse.Message);
		await botClient.SendTextMessageAsync(
			chatId, "Can't process message", ParseMode.Markdown, cancellationToken: cancellationToken);
		return Results.BadRequest();
	}

	logger.LogInformation("Successful response from chat {chatId}. Date: {dateTime}", 
		chatId.ToString(), DateTime.UtcNow);

	return Results.Ok();
});

app.Run();
using System;
using System.Threading.Tasks;
using Classes.Api.Handlers.UpdateHandler;
using Classes.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Api.Controllers;

[Route("api/v1/[controller]")]
public class MessageController : Controller
{
    private IUpdateHandler? UpdateHandler { get; set; }
    private IUnitOfWork Services { get; }
    private ILogger<MessageController> Logger { get; }
    private ITelegramBotClient TelegramBotClient { get; }
    private UpdateHandlerResponse UpdateHandlerResponse { get; set; } = new();

    public MessageController(
        ITelegramBotClient telegramBotClient, 
        IUnitOfWork services, 
        ILogger<MessageController> logger)
    {
        TelegramBotClient = telegramBotClient;
        Services = services;
        Logger = logger;
    }

    [HttpPost("update")]
    public async Task<IActionResult> MessageUpdate([FromBody] Update update)
    {
        UpdateHandler = GetUpdateHandler(update);

        try
        {
            if (UpdateHandler is not null)
                await UpdateHandler.Handle(update);
        }
        catch (Exception exception)
        {
            UpdateHandlerResponse = new UpdateHandlerResponse {Message = $"Exception:\n{exception}"};
        }

        if (UpdateHandlerResponse.ResponseType.IsError())
        {
            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery!.From.Id;
            Logger.LogError("Chat id:\n{ChatId}\nError message:\n{ErrorMessage}",
                chatId, UpdateHandlerResponse.Message);
            await TelegramBotClient.SendTextMessageAsync(chatId, "Can't process message.", ParseMode.Markdown);
        }

        Logger.LogInformation("Successful response. {DateTime}", DateTime.UtcNow);
        return Ok();
    }

    private IUpdateHandler? GetUpdateHandler(Update update) =>
        update switch
        {
            {Type: UpdateType.Message, Message.Type: MessageType.Location} =>
                new LocationUpdateHandler(TelegramBotClient, Services),
            {Type: UpdateType.Message} => new MessageUpdateHandler(TelegramBotClient, Services),
            {Type: UpdateType.CallbackQuery} => new CallbackQueryUpdateHandler(TelegramBotClient, Services),
            {Type: UpdateType.Unknown} => null,
            {Type: UpdateType.InlineQuery} => null,
            {Type: UpdateType.ChosenInlineResult} => null,
            {Type: UpdateType.EditedMessage} => null,
            {Type: UpdateType.ChannelPost} => null,
            {Type: UpdateType.EditedChannelPost} => null,
            {Type: UpdateType.ShippingQuery} => null,
            {Type: UpdateType.PreCheckoutQuery} => null,
            {Type: UpdateType.Poll} => null,
            {Type: UpdateType.PollAnswer} => null,
            _ => null
        };
}
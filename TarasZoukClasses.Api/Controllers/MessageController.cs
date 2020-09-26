namespace TarasZoukClasses.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Domain.Handlers.UpdateHandler;
    using Domain.Handlers.UpdateHandler.UpdateHandlerContract;
    using Domain.Handlers.UpdateHandlerResponse;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    [Route("api/v1")]
    public class MessageController : Controller
    {
        private ILogger<MessageController> Logger { get; }

        private TelegramBotClient TelegramBotClient { get; }

        private IUpdateHandler UpdateHandler { get; set; }

        private UpdateHandlerResponse UpdateHandlerResponse { get; set; }

        public MessageController(ILogger<MessageController> logger, TelegramBotClient telegramBotClient)
        {
            Logger = logger;
            TelegramBotClient = telegramBotClient;
        }

        [HttpPost]
        [Route("message/update")]
        public async Task<IActionResult> MessageUpdate([FromBody] Update update)
        {
            UpdateHandler = update.Type switch
            {
                UpdateType.Message => new MessageUpdateHandler(TelegramBotClient),
                UpdateType.CallbackQuery => new CallbackQueryUpdateHandler(TelegramBotClient),
                UpdateType.Unknown => throw new NotImplementedException(),
                UpdateType.InlineQuery => throw new NotImplementedException(),
                UpdateType.ChosenInlineResult => throw new NotImplementedException(),
                UpdateType.EditedMessage => throw new NotImplementedException(),
                UpdateType.ChannelPost => throw new NotImplementedException(),
                UpdateType.EditedChannelPost => throw new NotImplementedException(),
                UpdateType.ShippingQuery => throw new NotImplementedException(),
                UpdateType.PreCheckoutQuery => throw new NotImplementedException(),
                UpdateType.Poll => throw new NotImplementedException(),
                UpdateType.PollAnswer => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };

            try
            {
                UpdateHandlerResponse = await UpdateHandler.Handle(update);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception.Message, exception);
                return BadRequest(exception.Message);
            }

            if (UpdateHandlerResponse.ResponseType.IsError())
            {
                Logger.LogError(UpdateHandlerResponse.Message);
                return BadRequest(UpdateHandlerResponse.Message);
            }

            Logger.LogInformation($"Successful response. {DateTime.UtcNow}.");
            return Ok();
        }
    }
}

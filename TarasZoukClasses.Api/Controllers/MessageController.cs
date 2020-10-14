namespace TarasZoukClasses.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Domain.Handlers.UpdateHandler;
    using Domain.Handlers.UpdateHandler.UpdateHandlerContract;
    using Domain.Handlers.UpdateHandlerResponse;
    using Domain.Service.BaseService;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    [Route("api/v1")]
    public class MessageController : Controller
    {
        private IUpdateHandler UpdateHandler { get; set; }

        private IUnitOfWork Services { get; }

        private ILogger<MessageController> Logger { get; }

        private TelegramBotClient TelegramBotClient { get; }

        private UpdateHandlerResponse UpdateHandlerResponse { get; }

        public MessageController(TelegramBotClient telegramBotClient, IUnitOfWork services, ILogger<MessageController> logger)
        {
            TelegramBotClient = telegramBotClient;
            Services = services;
            Logger = logger;
            UpdateHandlerResponse = new UpdateHandlerResponse();
        }

        [HttpPost]
        [Route("message/update")]
        public async Task<IActionResult> MessageUpdate([FromBody] Update update)
        {
            UpdateHandler = update.Type switch
            {
                UpdateType.Message => new MessageUpdateHandler(TelegramBotClient, Services),
                UpdateType.CallbackQuery => new CallbackQueryUpdateHandler(TelegramBotClient, Services),
                UpdateType.Unknown => null,
                UpdateType.InlineQuery => null,
                UpdateType.ChosenInlineResult => null,
                UpdateType.EditedMessage => null,
                UpdateType.ChannelPost => null,
                UpdateType.EditedChannelPost => null,
                UpdateType.ShippingQuery => null,
                UpdateType.PreCheckoutQuery => null,
                UpdateType.Poll => null,
                UpdateType.PollAnswer => null,
                _ => null
            };

            try
            {
                if (UpdateHandler != null) await UpdateHandler.Handle(update);
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

namespace TarasZoukClasses.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Domain.Service.BaseService;
    using Handlers.UpdateHandler;
    using Handlers.UpdateHandlerResponse;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using CallbackQueryUpdateHandler = Handlers.UpdateHandler.CallbackQueryUpdateHandler;
    using IUpdateHandler = Handlers.UpdateHandler.UpdateHandlerContract.IUpdateHandler;
    using MessageUpdateHandler = Handlers.UpdateHandler.MessageUpdateHandler;
    using UpdateHandlerResponse = Handlers.UpdateHandlerResponse.UpdateHandlerResponse;

    [Route("api/v1")]
    public class MessageController : Controller
    {
        private IUpdateHandler UpdateHandler { get; set; }

        private IUnitOfWork Services { get; }

        private ILogger<MessageController> Logger { get; }

        private TelegramBotClient TelegramBotClient { get; }

        private UpdateHandlerResponse UpdateHandlerResponse { get; set; }

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
            UpdateHandler = update switch
            {
                { Type: UpdateType.Message, Message: { Type: MessageType.Location } } => new LocationUpdateHandler(TelegramBotClient, Services),
                { Type: UpdateType.Message } => new MessageUpdateHandler(TelegramBotClient, Services),
                { Type: UpdateType.CallbackQuery } => new CallbackQueryUpdateHandler(TelegramBotClient, Services),
                { Type: UpdateType.Unknown } => null,
                { Type: UpdateType.InlineQuery } => null,
                { Type: UpdateType.ChosenInlineResult } => null,
                { Type: UpdateType.EditedMessage } => null,
                { Type: UpdateType.ChannelPost } => null,
                { Type: UpdateType.EditedChannelPost } => null,
                { Type: UpdateType.ShippingQuery } => null,
                { Type: UpdateType.PreCheckoutQuery } => null,
                { Type: UpdateType.Poll } => null,
                { Type: UpdateType.PollAnswer } => null,
                _ => null
            };

            try
            {
                if (UpdateHandler != null)
                    UpdateHandlerResponse = await UpdateHandler.Handle(update);
            }
            catch (Exception exception)
            {
                UpdateHandlerResponse = new UpdateHandlerResponse
                {
                    Message = $"Exception:\n{exception}",
                    ResponseType = UpdateHandlerResponseType.Error
                };
            }

            if (UpdateHandlerResponse.ResponseType.IsError())
            {
                var chatId = update.Message?.Chat.Id ?? update.CallbackQuery.From.Id;
                Logger.LogError($"Chat id:\n{chatId}\nError message:\n{UpdateHandlerResponse.Message}");
                await TelegramBotClient.SendTextMessageAsync(chatId, "Can't process message.", ParseMode.Markdown);
            }

            Logger.LogInformation($"Successful response. {DateTime.UtcNow}.");
            return Ok();
        }
    }
}

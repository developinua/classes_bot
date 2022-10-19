// using System;
// using System.Threading.Tasks;
// using Classes.Api.Extensions;
// using Classes.Api.Handlers.UpdateHandler;
// using Classes.Domain.Repositories;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Telegram.Bot;
// using Telegram.Bot.Types;
// using Telegram.Bot.Types.Enums;
//
// namespace Classes.Api.Controllers;
//
// [Route("api/v1/[controller]")]
// public class MessageController : Controller
// {
//     private IUnitOfWork Db { get; }
//     private ILogger<MessageController> Logger { get; }
//     private ITelegramBotClient BotClient { get; }
//     private UpdateHandlerResponse UpdateHandlerResponse { get; set; } = new();
//
//     public MessageController(
//         ITelegramBotClient telegramBotClient, 
//         IUnitOfWork services, 
//         ILogger<MessageController> logger)
//     {
//         BotClient = telegramBotClient;
//         Db = services;
//         Logger = logger;
//     }
//
//     [HttpPost("update")]
//     public async Task<IActionResult> MessageUpdate([FromBody] Update update)
//     {
//         await update.GetHandler(BotClient, Db)?.Handle(update);
//         
//         var chatId = update.Message?.Chat.Id ?? update.CallbackQuery!.From.Id;
//
//         if (UpdateHandlerResponse.ResponseType.IsError())
//         {
//             Logger.LogError("Chat id:\n{chatId}\nError message:\n{errorMessage}",
//                 chatId.ToString(), UpdateHandlerResponse.Message);
//             await BotClient.SendTextMessageAsync(chatId, "Can't process message.", ParseMode.Markdown);
//         }
//
//         Logger.LogInformation("Successful response from chat {chatId}. {dateTime}", 
//             chatId.ToString(), DateTime.UtcNow);
//         return Ok();
//     }
// }
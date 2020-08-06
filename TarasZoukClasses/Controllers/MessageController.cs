using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TarasZoukClasses.TelegramBotModels;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TarasZoukClasses.Controllers
{
    [Route("api/v1")]
    public class MessageController : Controller
    {
        private ILogger<MessageController> Logger { get; }

        private TelegramBotClient TelegramBotClient { get; }

        private TelegramBot TelegramBot { get; }

        public MessageController(ILogger<MessageController> logger, TelegramBotClient telegramBotClient, TelegramBot telegramBot)
        {
            Logger = logger;
            TelegramBotClient = telegramBotClient;
            TelegramBot = telegramBot;
        }

        [HttpPost]
        [Route("message/update")]
        public async Task<IActionResult> MessageUpdate([FromBody] Update update)
        {
            if (update == null)
            {
                Logger.LogError($"Message update is null. {DateTime.UtcNow}.");
                return BadRequest();
            }

            Logger.LogInformation($"Trying to get active commands start. {DateTime.UtcNow}.");
            var commands = (await TelegramBot.GetActiveCommandsAsync()).ToList();
            Logger.LogInformation($"Trying to get active commands finished. {DateTime.UtcNow}.");

            var message = update.Message;

            foreach (var command in commands.Where(command => command.Contains(message)))
            {
                await command.Execute(message, TelegramBotClient);
                break;
            }

            Logger.LogInformation($"Successful response. {DateTime.UtcNow}.");
            return Ok();
        }
    }
}

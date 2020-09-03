namespace TarasZoukClasses.Api.TelegramBotModels
{
    using Commands;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class TelegramBot
    {
        public async Task<IEnumerable<ICommand>> GetActiveCommandsAsync(ILogger<Controllers.MessageController> logger)
        {
            logger.LogInformation($"Trying to get active commands start. {DateTime.UtcNow}.");
            // TODO: Add possibility to get active commands from db and via reflection return active commands.
            var commands = await Task.Run(() => new List<ICommand>
            {
                new StartCommand()
            });
            logger.LogInformation($"Trying to get active commands finished. {DateTime.UtcNow}.");

            return commands;
        }
    }
}

namespace TarasZoukClasses.Models.TelegramBotModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TarasZoukClasses.Models.TelegramBotModels.Commands;

    public class TelegramBot
    {
        public static async Task<IEnumerable<ICommand>> GetActiveCommandsAsync()
        {
            //logger.LogInformation($"Trying to get active commands start. {DateTime.UtcNow}.");
            // TODO: Add possibility to get active commands from db and via reflection return active commands.
            var commands = await Task.Run(() => new List<ICommand>
            {
                new StartCommand()
            });
            //logger.LogInformation($"Trying to get active commands finished. {DateTime.UtcNow}.");

            return commands;
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using TarasZoukClasses.TelegramBotModels.Commands;

namespace TarasZoukClasses.TelegramBotModels
{
    public class TelegramBot
    {
        public async Task<IEnumerable<ICommand>> GetActiveCommandsAsync()
        {
            // TODO: Add possibility to get active commands from db and via reflection return active commands.
            return new List<ICommand>
            {
                new StartCommand()
            };
        }
    }
}

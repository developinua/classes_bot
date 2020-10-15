namespace TarasZoukClasses.Domain.Service.CommandService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Commands.Administration;
    using Commands.Administration.Admin;
    using Commands.Administration.Seed;
    using Commands.CheckIn;
    using Commands.Contract;
    using Commands.MySubscriptions;
    using Commands.Start;
    using Data.Context;
    using Data.Models;
    using Data.Repositories;

    public class CommandMongoDbRepository : MongoDbRepository<Command>, ICommandRepository
    {
        public CommandMongoDbRepository(IMongoDbContext context) : base(context) { }

        public async Task<IEnumerable<ICommand>> GetActiveCommandsAsync()
        {
            //logger.LogInformation($"Trying to get active commands start. {DateTime.UtcNow}.");
            // TODO: Add possibility to get active commands from db and via reflection return active commands.
            var commands = await Task.Run(() => new List<ICommand>
            {
                new StartCommand(),
                new MySubscriptionsCommand(),
                new CheckInCommand(),
                new SeedCommand(),
                new AdminCommand()
            });
            //logger.LogInformation($"Trying to get active commands finished. {DateTime.UtcNow}.");

            return commands;
        }
    }
}

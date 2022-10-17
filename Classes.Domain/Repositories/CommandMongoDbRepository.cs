using System.Collections.Generic;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Commands;
using Classes.Domain.Commands.Administration.Admin;
using Classes.Domain.Commands.Administration.Seed;
using Classes.Domain.Commands.CheckIn;
using Classes.Domain.Commands.MySubscriptions;
using Classes.Domain.Commands.Start;
using Classes.Domain.Repositories.Base;

namespace Classes.Domain.Repositories;

public interface ICommandRepository : IGenericReadonlyRepository<Command>
{
    Task<IEnumerable<IBotCommand>> GetActiveCommandsAsync();
}

public class CommandMongoDbRepository : MongoDbRepository<Command>, ICommandRepository
{
    public CommandMongoDbRepository(IMongoDbContext context) : base(context) { }

    public async Task<IEnumerable<IBotCommand>> GetActiveCommandsAsync()
    {
        //logger.LogInformation($"Trying to get active commands start. {DateTime.UtcNow}.");
        // TODO: Add possibility to get active commands from db and via reflection return active commands.
        var commands = await Task.Run(() => new List<IBotCommand>
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Classes.Domain.Commands;

namespace Classes.Domain.Repositories;

public interface ICommandRepository
{
    Task<IEnumerable<IBotCommand>> GetActiveCommandsAsync();
}

public class CommandRepository : ICommandRepository
{
    private readonly IEnumerable<IBotCommand> _botCommands;
    public CommandRepository(IEnumerable<IBotCommand> botCommands) => _botCommands = botCommands;

    public Task<IEnumerable<IBotCommand>> GetActiveCommandsAsync() => Task.FromResult(_botCommands);
}
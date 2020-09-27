namespace TarasZoukClasses.Domain.Service.CommandService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Models;

    public class CommandMongoDbRepository : ICommandRepository
    {
        public async Task<Command> Get(int id)
        {
            //logger.LogInformation($"Trying to get active commands start. {DateTime.UtcNow}.");
            // TODO: Add possibility to get active commands from db and via reflection return active commands.
            //var commands = await Task.Run(() => new List<ICommand>
            //{
            //    new StartCommand()
            //});
            //logger.LogInformation($"Trying to get active commands finished. {DateTime.UtcNow}.");

            //return commands;
            return await Task.Run(() => new Command());
        }

        public Task<IEnumerable<Command>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public Task Add(Command entity)
        {
            throw new System.NotImplementedException();
        }

        public Task Update(Command entity)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}

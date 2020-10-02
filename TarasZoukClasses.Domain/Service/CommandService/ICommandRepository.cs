namespace TarasZoukClasses.Domain.Service.CommandService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Commands.Contract;
    using Data.Models;
    using Data.Repositories;

    public interface ICommandRepository : IGenericRepository<Command>
    {
        Task<IEnumerable<ICommand>> GetActiveCommandsAsync();
    }
}

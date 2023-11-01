using System;
using System.Threading.Tasks;
using Classes.Data.Context;

namespace Classes.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; set; }
    IUserInformationRepository UsersInformation { get; set; }
    ISubscriptionRepository Subscriptions { get; set; }
    IUserSubscriptionRepository UsersSubscriptions { get; set; }
    ICultureRepository Cultures { get; set; }
    ICommandRepository Commands { get; set; }
    
    Task<int> SaveChanges();
}

public class UnitOfWork : IUnitOfWork
{
    private IMongoDbContext Context { get; }
    public IUserRepository Users { get; set; }
    public IUserInformationRepository UsersInformation { get; set; }
    public ISubscriptionRepository Subscriptions { get; set; }
    public IUserSubscriptionRepository UsersSubscriptions { get; set; }
    public ICultureRepository Cultures { get; set; }
    public ICommandRepository Commands { get; set; }

    public UnitOfWork(
        IMongoDbContext context,
        IUserRepository usersRepository,
        IUserInformationRepository usersInformation,
        ISubscriptionRepository subscriptions,
        IUserSubscriptionRepository usersSubscriptions,
        ICultureRepository cultures,
        ICommandRepository commandRepository)
    {
        Context = context;
        Users = usersRepository;
        UsersInformation = usersInformation;
        Subscriptions = subscriptions;
        UsersSubscriptions = usersSubscriptions;
        Cultures = cultures;
        Commands = commandRepository;
    }

    public async Task<int> SaveChanges() => await Context.SaveChanges();

    public void Dispose() => Context.Dispose();
}
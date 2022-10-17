using System;
using System.Threading.Tasks;
using Classes.Data.Context;

namespace Classes.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IZoukUserRepository ZoukUsers { get; set; }

    IZoukUserAdditionalInformationRepository ZoukUsersAdditionalInformation { get; set; }

    ISubscriptionRepository Subscriptions { get; set; }

    IZoukUserSubscriptionRepository ZoukUsersSubscriptions { get; set; }

    ICultureRepository Cultures { get; set; }

    ICommandRepository Commands { get; set; }

    Task<int> SaveChanges();
}

public class UnitOfWork : IUnitOfWork
{
    private IMongoDbContext Context { get; }

    public IZoukUserRepository ZoukUsers { get; set; }

    public IZoukUserAdditionalInformationRepository ZoukUsersAdditionalInformation { get; set; }

    public ISubscriptionRepository Subscriptions { get; set; }

    public IZoukUserSubscriptionRepository ZoukUsersSubscriptions { get; set; }

    public ICultureRepository Cultures { get; set; }

    public ICommandRepository Commands { get; set; }

    public UnitOfWork(IMongoDbContext context,
        IZoukUserRepository zoukUsersRepository,
        IZoukUserAdditionalInformationRepository zoukUsersAdditionalInformation,
        ISubscriptionRepository subscriptions,
        IZoukUserSubscriptionRepository zoukUsersSubscriptions,
        ICultureRepository cultures,
        ICommandRepository commandRepository)
    {
        Context = context;
        ZoukUsers = zoukUsersRepository;
        ZoukUsersAdditionalInformation = zoukUsersAdditionalInformation;
        Subscriptions = subscriptions;
        ZoukUsersSubscriptions = zoukUsersSubscriptions;
        Cultures = cultures;
        Commands = commandRepository;
    }

    public async Task<int> SaveChanges()
    {
        return await Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
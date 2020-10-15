namespace TarasZoukClasses.Domain.Service.BaseService
{
    using System.Threading.Tasks;
    using CommandService;
    using CultureService;
    using Data.Context;
    using SubscriptionService;
    using ZoukUserAdditionalInformationService;
    using ZoukUserService;
    using ZoukUserSubscription;

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
}

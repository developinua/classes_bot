namespace TarasZoukClasses.Api.Configurations
{
    using Data.Context;
    using Domain.Service.BaseService;
    using Domain.Service.CommandService;
    using Domain.Service.CultureService;
    using Domain.Service.SubscriptionService;
    using Domain.Service.ZoukUserAdditionalInformationService;
    using Domain.Service.ZoukUserService;
    using Domain.Service.ZoukUserSubscription;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesConfiguration
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IMongoDbContext, MongoDbContext>();

            services.AddTransient<IZoukUserRepository, ZoukUserMongoDbRepository>();
            services.AddTransient<IZoukUserAdditionalInformationRepository, ZoukUserAdditionalInformationMongoDbRepository>();
            services.AddTransient<ISubscriptionRepository, SubscriptionMongoDbRepository>();
            services.AddTransient<IZoukUserSubscriptionRepository, ZoukUserSubscriptionMongoDbRepository>();
            services.AddTransient<ICultureRepository, CultureMongoDbRepository>();
            services.AddTransient<ICommandRepository, CommandMongoDbRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}

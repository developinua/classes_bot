namespace TarasZoukClasses.Api.Configurations
{
    using Data.Context;
    using Domain.Service.BaseService;
    using Domain.Service.CommandService;
    using Domain.Service.PaymentService;
    using Domain.Service.UserPaymentService;
    using Domain.Service.UserService;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesConfiguration
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IMongoDbContext, MongoDbContext>();

            services.AddTransient<IUserRepository, UserMongoDbRepository>();
            services.AddTransient<IPaymentRepository, PaymentMongoDbRepository>();
            services.AddTransient<IUserPaymentRepository, UserPaymentMongoDbRepository>();
            services.AddTransient<ICommandRepository, CommandMongoDbRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}

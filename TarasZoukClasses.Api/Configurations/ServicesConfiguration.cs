namespace TarasZoukClasses.Api.Configurations
{
    using Data.Context;
    using Domain.Service.BaseService;
    using Domain.Service.CommandService;
    using Domain.Service.PaymentService;
    using Domain.Service.UserService;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesConfiguration
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserMongoDbRepository>();
            services.AddTransient<IPaymentRepository, PaymentMongoDbRepository>();
            services.AddTransient<ICommandRepository, CommandMongoDbRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IMongoDbContext, MongoDbContext>();

            //services.AddDbContext<IMongoDbContext>(opt => opt
            //    .UseSqlServer("Server=localhost,1433; Database=BooksDB;User Id=sa; Password=password_01;"));

            return services;
        }
    }
}

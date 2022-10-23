using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using Classes.Domain.Models.Settings;
using Classes.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Classes.Domain.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoConnectionSettings>(configuration.GetSection(MongoConnectionSettings.Position));
        
        services.AddScoped<IMongoDbContext, MongoDbContext>();

        services.AddScoped<IUserRepository, UserMongoDbRepository>();
        services.AddScoped<IUserInformationRepository, UserInformationMongoDbRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionMongoDbRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionMongoDbRepository>();
        services.AddScoped<ICultureRepository, CultureMongoDbRepository>();
        services.AddScoped<ICommandRepository, CommandMongoDbRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
    
    public static async Task UseTelegramBotWebHooks(this IServiceCollection services, IConfiguration configuration)
    {
        TelegramBotSettings telegramBotSettings = new();
        configuration.GetSection(TelegramBotSettings.Position).Bind(telegramBotSettings);

        ITelegramBotClient telegramBotClient = new TelegramBotClient(telegramBotSettings.Token);
        var hook = string.Concat(telegramBotSettings.Url, telegramBotSettings.UpdateRoute);

        services.AddSingleton(telegramBotClient);

        await telegramBotClient.SetWebhookAsync(hook);
    }
}
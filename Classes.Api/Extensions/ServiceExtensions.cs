using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Models.Settings;
using Classes.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Classes.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddTransient<IMongoDbContext, MongoDbContext>();

        services.AddTransient<IUserRepository, UserMongoDbRepository>();
        services.AddTransient<IUserInformationRepository, UserInformationMongoDbRepository>();
        services.AddTransient<ISubscriptionRepository, SubscriptionMongoDbRepository>();
        services.AddTransient<IUserSubscriptionRepository, UserSubscriptionMongoDbRepository>();
        services.AddTransient<ICultureRepository, CultureMongoDbRepository>();
        services.AddTransient<ICommandRepository, CommandMongoDbRepository>();

        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }
    
    public static async Task UseTelegramBotWebHooks(this IServiceCollection services, IConfiguration configuration)
    {
        var telegramBotSettings = new TelegramBotSettings();
        configuration.GetSection(TelegramBotSettings.Position).Bind(telegramBotSettings);

        var telegramBotClient = new TelegramBotClient(telegramBotSettings.Token);
        var hook = string.Concat(telegramBotSettings.Url, telegramBotSettings.UpdateRoute);

        services.AddSingleton(telegramBotClient);

        await telegramBotClient.SetWebhookAsync(hook);
    }
}
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Models.Settings;
using Classes.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Classes.Domain.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostgresDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileEditableRepository>();
        services.AddScoped<ISubscriptionEditableRepository, SubscriptionRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        services.AddScoped<ICultureRepository, CultureRepository>();
        services.AddScoped<ICommandRepository, CommandRepository>();

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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Repositories;
using Classes.Domain.Handlers.Start;
using Classes.Domain.Mapper;
using Classes.Domain.Models.BotRequest;
using Classes.Domain.Models.Settings;
using Classes.Domain.Services;
using Classes.Domain.Validators;
using FluentValidation;
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
        
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<StartHandler>());
        
        services.AddScoped<IBotService, BotService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUpdateService, UpdateService>();
        services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileEditableRepository>();
        services.AddScoped<ISubscriptionEditableRepository, SubscriptionRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        services.AddScoped<ICultureRepository, CultureRepository>();
        services.AddScoped<ICommandRepository, CommandRepository>();
        
        services.AddAutoMapper(typeof(UpdateProfile));
        services.AddValidatorsFromAssemblyContaining<MessageValidator>();
        
        // Register all classes that is assigned from bot requests
        var botRequests = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                (
                    typeof(BotMessageRequest).IsAssignableFrom(t)
                    || typeof(BotCallbackRequest).IsAssignableFrom(t)
                )
                && t is { IsInterface: false, IsAbstract: false });

        foreach (var callbackRequest in botRequests)
        {
            if (callbackRequest.IsAssignableTo(typeof(BotMessageRequest)))
                services.AddScoped(typeof(BotMessageRequest), callbackRequest);
            else if (callbackRequest.IsAssignableTo(typeof(BotCallbackRequest)))
                services.AddScoped(typeof(BotCallbackRequest), callbackRequest);
        }

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
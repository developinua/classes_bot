using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Classes.Application.Handlers.Start;
using Classes.Application.Services;
using Classes.Data.Context;
using Classes.Data.Repositories;
using Classes.Domain.BotRequest;
using Classes.Domain.Mapper;
using Classes.Domain.Models.Settings;
using Classes.Domain.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Classes.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostgresDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<StartHandler>());
        
        services.AddScoped<IBotService, BotService>();
        services.AddScoped<IUpdateService, UpdateService>();
        services.AddScoped<IReplyMarkupService, ReplyMarkupService>();
        services.AddScoped<ICallbackExtractorService, CallbackExtractorService>();
        
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ICultureService, CultureService>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        services.AddScoped<ICultureRepository, CultureRepository>();
        
        services.AddAutoMapper(typeof(UpdateProfile));
        services.AddValidatorsFromAssemblyContaining<MessageValidator>();
        
        // Register all classes that is assigned from bot requests
        var botRequests = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => 
                (typeof(BotMessageRequest).IsAssignableFrom(t)
                 || typeof(BotCallbackRequest).IsAssignableFrom(t))
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
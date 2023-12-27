using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Classes.Application.Handlers.Exceptions;
using Classes.Application.Handlers.Start;
using Classes.Application.Middleware;
using Classes.Application.Providers;
using Classes.Application.Services;
using Classes.Data.Context;
using Classes.Data.Repositories;
using Classes.Domain.Mapper;
using Classes.Domain.Models.Settings;
using Classes.Domain.Requests.Bot;
using Classes.Domain.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Classes.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IBotService, BotService>();
        services.AddScoped<IUpdateService, UpdateService>();
        services.AddScoped<IReplyMarkupService, ReplyMarkupService>();
        services.AddScoped<ICallbackExtractorService, CallbackExtractorService>();
        
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<ICultureService, CultureService>();
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        services.AddScoped<ICultureRepository, CultureRepository>();
        
        return services;
    }

    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddScoped<UsernameMustBeFilledInMiddleware>();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostgresDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
    
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<StartHandler>());
        return services;
    }
    
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UpdateProfile));
        services.AddValidatorsFromAssemblyContaining<MessageValidator>();
        
        return services;
    }
    
    public static IServiceCollection AddBotRequests(this IServiceCollection services)
    {
        // Register all classes that is assigned from bot requests
        var botRequests = Assembly.GetAssembly(typeof(BotMessageRequest))!
            .GetTypes()
            .Where(t =>
                (typeof(BotMessageRequest).IsAssignableFrom(t)
                 || typeof(BotCallbackRequest).IsAssignableFrom(t))
                && t is { IsInterface: false, IsAbstract: false })
            .ToList();

        foreach (var callbackRequest in botRequests)
        {
            if (callbackRequest.IsAssignableTo(typeof(BotMessageRequest)))
                services.AddScoped(typeof(BotMessageRequest), callbackRequest);
            else if (callbackRequest.IsAssignableTo(typeof(BotCallbackRequest)))
                services.AddScoped(typeof(BotCallbackRequest), callbackRequest);
        }

        return services;
    }
    
    public static IServiceCollection AddCustomLocalizations(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("uk-UA")
            };
            
            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.ApplyCurrentCultureToResponseHeaders = true;
            options.RequestCultureProviders.Insert(0, new UpdateRequestCultureProvider());
        });

        return services;
    }
    
    public static IApplicationBuilder UseCustomRequestLocalization(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()!.Value;
        app.UseRequestLocalization(options);

        return app;
    }
    
    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<UsernameMustBeFilledInMiddleware>();

        return app;
    }

    public static async Task SetTelegramBotWebHook(this IServiceCollection services, IConfiguration configuration)
    {
        TelegramBotSettings telegramBotSettings = new();
        configuration.GetSection(TelegramBotSettings.Position).Bind(telegramBotSettings);

        ITelegramBotClient telegramBotClient = new TelegramBotClient(telegramBotSettings.Token);
        var hook = string.Concat(telegramBotSettings.Url, telegramBotSettings.UpdateRoute);

        services.AddSingleton(telegramBotClient);

        await telegramBotClient.SetWebhookAsync(hook);
    }
}
namespace TarasZoukClasses.Models.Configurations
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using TarasZoukClasses.Models.TelegramBotModels;
    using Telegram.Bot;

    public static class TelegramBotConfiguration
    {
        public static async Task UseTelegramBotWebHooks(this IServiceCollection services, IConfiguration configuration)
        {
            var telegramBotSettings = new TelegramBotSettings();

            configuration.GetSection(TelegramBotSettings.AppSettingsName)
                .Bind(telegramBotSettings);

            var telegramBotClient = new TelegramBotClient(telegramBotSettings.Token);
            var hook = string.Concat(telegramBotSettings.Url, telegramBotSettings.UpdateRoute);

            services.AddSingleton(telegramBotClient);
            services.AddScoped<TelegramBot>();

            await telegramBotClient.SetWebhookAsync(hook);
        }
    }
}

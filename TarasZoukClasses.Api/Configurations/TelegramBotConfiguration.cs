namespace TarasZoukClasses.Api.Configurations
{
    using System.Threading.Tasks;
    using Domain.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
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

            await telegramBotClient.SetWebhookAsync(hook);
        }
    }
}

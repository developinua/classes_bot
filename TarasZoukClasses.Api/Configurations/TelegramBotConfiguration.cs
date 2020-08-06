using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using TarasZoukClasses.TelegramBotModels;
using Telegram.Bot;

namespace TarasZoukClasses.Configurations
{
    public static class TelegramBotConfiguration
    {
        public static async Task UseTelegramBotWebHooks(this IServiceCollection services, IConfiguration configuration)
        {
            var telegramBotSettings = new BotSettings();
            configuration.GetSection(BotSettings.AppSettingsName)
                .Bind(telegramBotSettings);

            var botClient = new TelegramBotClient(telegramBotSettings.Token);
            var hook = string.Concat(telegramBotSettings.Url, telegramBotSettings.UpdateRoute);

            services.AddSingleton(botClient);
            services.AddScoped<TelegramBot>();

            await botClient.SetWebhookAsync(hook);
        }
    }
}

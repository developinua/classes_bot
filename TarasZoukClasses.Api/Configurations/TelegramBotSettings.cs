namespace TarasZoukClasses.Api.Configurations
{
    public class TelegramBotSettings
    {
        public const string AppSettingsName = "TelegramBot";

        public string Url { get; set; }

        public string UpdateRoute { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }
    }
}

namespace TarasZoukClasses.TelegramBotModels
{
    public class BotSettings
    {
        public const string AppSettingsName = "TelegramBot";

        public string Url { get; set; }

        public string UpdateRoute { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }
    }
}

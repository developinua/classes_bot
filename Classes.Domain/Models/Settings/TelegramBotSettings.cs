namespace Classes.Domain.Models.Settings;

public class TelegramBotSettings
{
    public const string Position = "TelegramBot";

    public string Url { get; set; }
    public string UpdateRoute { get; set; }
    public string Name { get; set; }
    public string Token { get; set; }
}
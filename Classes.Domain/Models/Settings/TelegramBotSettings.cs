namespace Classes.Domain.Models.Settings;

public class TelegramBotSettings
{
    public const string Position = "TelegramBot";

    public string Url { get; set; } = string.Empty;
    public string UpdateRoute { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
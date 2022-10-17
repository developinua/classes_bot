namespace Classes.Data.Models;

public class Culture : Document
{
    public string Name { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}
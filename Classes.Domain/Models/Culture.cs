namespace Classes.Domain.Models;

public class Culture(string name = "en", string languageCode = "en-us") : BaseModel
{
    public string Name { get; set; } = name;
    public string LanguageCode { get; set; } = languageCode;
}
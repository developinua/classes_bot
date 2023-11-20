namespace Classes.Data.Models;

public class Culture : BaseModel
{
    public string Name { get; set; }
    public string LanguageCode { get; set; }

    public Culture(string name = "en", string languageCode = "en-US")
    {
        Name = name;
        LanguageCode = languageCode;
    }
}
namespace Classes.Domain.Models;

public class Culture(string name = "English", string code = "en-US") : BaseModel
{
    public string Name { get; set; } = name;
    public string Code { get; set; } = code;
}
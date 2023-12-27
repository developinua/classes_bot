namespace Classes.Domain.Models;

public class Culture(string name = "en", string code = "en-US") : BaseModel
{
    public string Name { get; set; } = name;
    public string Code { get; set; } = code;
}
namespace Classes.Domain.Models;

public class User : BaseModel
{
    public string NickName { get; set; } = string.Empty;
    public UserProfile UserProfile { get; set; } = new();
}
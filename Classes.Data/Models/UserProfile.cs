namespace Classes.Data.Models;

public class UserProfile : BaseModel
{
    public long ChatId { get; set; }
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public bool IsPremium { get; set; }
    public bool IsBot { get; set; }
    public Culture Culture { get; set; } = new();
}
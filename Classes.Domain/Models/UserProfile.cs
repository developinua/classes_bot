namespace Classes.Domain.Models;

public class UserProfile : BaseModel
{
    public long ChatId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsPremium { get; set; }
    public bool IsBot { get; set; }
    public Culture Culture { get; set; } = null!;
}
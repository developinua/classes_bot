namespace Classes.Domain.Models;

public class UserSubscription : BaseModel
{
    public User User { get; set; } = new();
    public Subscription Subscription { get; set; } = new();
    public int RemainingClasses { get; set; }
}
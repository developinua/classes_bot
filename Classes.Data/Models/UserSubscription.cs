namespace Classes.Data.Models;

public class UserSubscription : Document
{
    public User User { get; set; } = new();
    public Subscription Subscription { get; set; } = new();
    public int RemainingClassesCount { get; set; }
}
namespace Classes.Data.Models;

public class ZoukUserSubscription : Document
{
    public User User { get; set; }
    public Subscription Subscription { get; set; }
    public int RemainingClassesCount { get; set; }
}
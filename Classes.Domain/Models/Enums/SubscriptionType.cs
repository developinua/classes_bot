using System.ComponentModel.DataAnnotations;

namespace Classes.Domain.Models.Enums;

// todo: extract to db
public enum SubscriptionType
{
    [Display(Name = "NoSubscription")]
    None = 0,
    [Display(Name = "Novice")]
    Novice,
    [Display(Name = "Intermediate")]
    Intermediate,
    [Display(Name = "LadyStyling")]
    LadyStyling,
    [Display(Name = "ManStyling")]
    ManStyling,
    [Display(Name = "Premium")]
    Premium
}
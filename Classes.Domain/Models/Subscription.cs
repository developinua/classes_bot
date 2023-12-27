using System.ComponentModel.DataAnnotations;
using Classes.Domain.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Classes.Domain.Models;

public class Subscription : BaseModel
{
    [StringLength(250)]
    public string Name { get; set; } = string.Empty;
    [StringLength(1000)]
    public string? Description { get; set; }
    [Range(0, int.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, 100)]
    public decimal DiscountPercent { get; set; }
    [Range(0, 100)]
    public int ClassesCount { get; set; }
    public bool IsActive { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public SubscriptionType Type { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public SubscriptionPeriod Period { get; set; }

    public decimal GetPriceWithDiscount() =>
        DiscountPercent == 0
            ? Price
            : Price - Price * DiscountPercent / 100;

    // todo: localize
    public override string ToString()
    {
        var priceText = DiscountPercent == 0
            ? $"Price: {Price}\n"
            : $"Price with discount: {GetPriceWithDiscount()}\n";

        return $"Subscription: {Name}\n" +
               $"{priceText}" +
               $"Description: {Description}\n" +
               $"SubscriptionType: {Type}\n" +
               $"Total Classes: {ClassesCount}\n";
    }
}
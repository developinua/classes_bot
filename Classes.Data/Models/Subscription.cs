﻿using Classes.Data.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Classes.Data.Models;

public class Subscription : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercent { get; set; }
    public int Classes { get; set; }
    public bool IsActive { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public SubscriptionType Type { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public SubscriptionPeriod Period { get; set; }

    public decimal GetPriceWithDiscount() =>
        DiscountPercent == 0
            ? Price
            : Price - Price * DiscountPercent / 100;

    public override string ToString()
    {
        var priceText = DiscountPercent == 0
            ? $"Price: {Price}\n"
            : $"Price with discount: {GetPriceWithDiscount()}\n";

        return $"Subscription: {Name}\n" +
               $"{priceText}" +
               $"Description: {Description}\n" +
               $"SubscriptionType: {Type}\n" +
               $"Total Classes: {Classes}\n";
    }
}
namespace TarasZoukClasses.Data.Models.Subscription
{
    using Base;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Subscription : Document
    {
        public string Name { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public int ClassesCount { get; set; }

        public bool IsActive { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public SubscriptionType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public SubscriptionPeriod Period { get; set; }

        public decimal GetSummaryPrice() => DiscountPercent == 0 ? Price : Price - Price * DiscountPercent / 100;

        public override string ToString()
        {
            var priceText = DiscountPercent == 0
                ? $"Price: {GetSummaryPrice()}\n"
                : $"Price with discount: {GetSummaryPrice()}\n";

            return $"Subscription: {Name}\n" +
                   $"{priceText}" +
                   $"Description: {Description}\n" +
                   $"SubscriptionType: {Type}\n" +
                   $"Total Classes: {ClassesCount}\n";
        }
    }
}

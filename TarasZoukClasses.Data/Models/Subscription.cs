namespace TarasZoukClasses.Data.Models
{
    using Base;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Subscription : Document
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public SubscriptionType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public SubscriptionPeriod Period { get; set; }

        public override string ToString()
        {
            return $"Subscription: {Name}\n" +
                   $"Price: {Price}\n" +
                   $"Description: {Description}\n" +
                   $"SubscriptionType: {Type}";
        }
    }
}

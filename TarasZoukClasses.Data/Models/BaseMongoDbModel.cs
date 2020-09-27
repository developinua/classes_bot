namespace TarasZoukClasses.Data.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public abstract class BaseMongoDbModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}

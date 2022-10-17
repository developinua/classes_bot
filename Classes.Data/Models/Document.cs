using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Classes.Data.Models;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    ObjectId Id { get; set; }
    DateTime CreatedAt { get; }
}

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public DateTime CreatedAt => Id.CreationTime;
    public override string ToString() => JsonConvert.SerializeObject(this);
}
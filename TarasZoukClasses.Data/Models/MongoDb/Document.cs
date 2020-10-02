namespace TarasZoukClasses.Data.Models.MongoDb
{
    using System;
    using MongoDB.Bson;
    using Newtonsoft.Json;

    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

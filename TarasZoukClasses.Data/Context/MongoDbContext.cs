namespace TarasZoukClasses.Data.Context
{
    using System;
    using Models;
    using MongoDB.Driver;

    public class MongoDbContext : IMongoDbContext
    {
        private IMongoDatabase MongoDatabase { get; }

        private MongoClient MongoClient { get; }

        public MongoDbContext()
        {
            var mongoDbSettings = new MongoDbSettings
            {
                Connection = Environment.GetEnvironmentVariable("MongoDbConnectionString"),
                DatabaseName = Environment.GetEnvironmentVariable("MongoDbDatabaseName")
            };
            MongoClient = new MongoClient(mongoDbSettings.Connection);
            MongoDatabase = MongoClient.GetDatabase(mongoDbSettings.DatabaseName);
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>(string name)
        {
            return MongoDatabase.GetCollection<TEntity>(name);
        }
    }
}

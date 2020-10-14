namespace TarasZoukClasses.Data.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Authentication;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    public class MongoDbContext : IMongoDbContext
    {
        private ILogger<MongoDbContext> Logger { get; }

        private IMongoDatabase MongoDatabase { get; }

        private MongoClient MongoClient { get; }

        private IClientSessionHandle Session { get; set; }

        private List<Func<Task>> Commands { get; }

        public MongoDbContext(ILogger<MongoDbContext> logger)
        {
            Logger = logger;
            var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(Environment.GetEnvironmentVariable("MongoDbConnectionString")));
            mongoClientSettings.SslSettings = new SslSettings
            {
                EnabledSslProtocols = SslProtocols.Tls12
            };

            MongoClient = new MongoClient(mongoClientSettings);
            MongoDatabase = MongoClient.GetDatabase(Environment.GetEnvironmentVariable("MongoDbDatabaseName"));
            Commands = new List<Func<Task>>();
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>(string name)
        {
            return MongoDatabase.GetCollection<TEntity>(name);
        }

        public void AddCommand(Func<Task> func)
        {
            Commands.Add(func);
        }

        public async Task<int> SaveChanges()
        {
            using (Session = await MongoClient.StartSessionAsync())
            {
                Session.StartTransaction();

                var commandTasks = Commands.Select(c => c());

                try
                {
                    await Task.WhenAll(commandTasks);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, exception.Message);
                    await Session.AbortTransactionAsync();
                }

                await Session.CommitTransactionAsync();
            }

            return Commands.Count;
        }

        public void Dispose()
        {
            while (Session != null && Session.IsInTransaction)
                Thread.Sleep(TimeSpan.FromMilliseconds(100));

            GC.SuppressFinalize(this);
        }
    }
}

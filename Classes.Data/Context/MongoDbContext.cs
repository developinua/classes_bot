using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Classes.Data.Context;

public interface IMongoDbContext : IDisposable
{
    IMongoCollection<TEntity> GetCollection<TEntity>(string name);
    void AddCommand(Func<Task> func);
    Task<int> SaveChanges();
}

public class MongoDbContext : IMongoDbContext
{
    private ILogger<MongoDbContext> Logger { get; }
    private IMongoDatabase MongoDatabase { get; }
    private MongoClient MongoClient { get; }
    private IClientSessionHandle Session { get; set; }
    private List<Func<Task>> Commands { get; } = new();

    public MongoDbContext(ILogger<MongoDbContext> logger, IClientSessionHandle session)
    {
        Logger = logger;
        Session = session;
        
        var mongoClientSettings = MongoClientSettings.FromUrl(
            new MongoUrl(Environment.GetEnvironmentVariable("MongoDbConnectionString")));
        mongoClientSettings.SslSettings = new SslSettings
        {
            EnabledSslProtocols = SslProtocols.Tls12
        };

        MongoClient = new MongoClient(mongoClientSettings);
        MongoDatabase = MongoClient.GetDatabase(Environment.GetEnvironmentVariable("MongoDbDatabaseName"));
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>(string name) => 
        MongoDatabase.GetCollection<TEntity>(name);

    public void AddCommand(Func<Task> func)
    {
        Commands.Add(func);
    }

    public async Task<int> SaveChanges()
    {
        using IClientSessionHandle? clientSessionHandle = Session = await MongoClient.StartSessionAsync();
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

        return Commands.Count;
    }

    public void Dispose()
    {
        while (Session is {IsInTransaction: true})
            Thread.Sleep(TimeSpan.FromMilliseconds(100));

        GC.SuppressFinalize(this);
    }
}
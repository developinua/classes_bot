using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
	private IClientSessionHandle? Session { get; set; }
	private ILogger<MongoDbContext> Logger { get; }
	private IMongoDatabase MongoDatabase { get; }
	private MongoClient MongoClient { get; }
	private List<Func<Task>> Commands { get; } = new();

	public MongoDbContext(
		ILogger<MongoDbContext> logger,
		IOptions<MongoConnectionSettings> mongoConnectionSettingsOptions)
	{
		MongoClient = new MongoClient(mongoConnectionSettingsOptions.Value.ConnectionString);
		MongoDatabase = MongoClient.GetDatabase(mongoConnectionSettingsOptions.Value.DatabaseName);
		Logger = logger;
	}

	public IMongoCollection<TEntity> GetCollection<TEntity>(string name) => MongoDatabase.GetCollection<TEntity>(name);

	public void AddCommand(Func<Task> func)
	{
		Commands.Add(func);
	}

	public async Task<int> SaveChanges()
	{
		using IClientSessionHandle clientSessionHandle = Session = await MongoClient.StartSessionAsync();
		clientSessionHandle.StartTransaction();

		var commandTasks = Commands.Select(c => c());

		try
		{
			await Task.WhenAll(commandTasks);
		}
		catch (Exception exception)
		{
			Logger.LogError(exception, "{errorMessage}", exception.Message);
			await clientSessionHandle.AbortTransactionAsync();
		}

		await clientSessionHandle.CommitTransactionAsync();

		return Commands.Count;
	}

	public void Dispose()
	{
		while (Session is {IsInTransaction: true})
			Thread.Sleep(TimeSpan.FromMilliseconds(100));

		GC.SuppressFinalize(this);
	}
}
namespace Classes.Domain.Models.Settings;

public class MongoConnectionSettings
{
	public const string Position = "MongoConnection";
	
	public string MongoConnectionString { get; set; }
	public string MongoDbConnectionString { get; set; }
	public string MongoDbDatabaseName { get; set; }
}
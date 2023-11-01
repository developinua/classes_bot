namespace Classes.Data.Models;

public class MongoConnectionSettings
{
	public const string Position = "MongoSettings";

	public string ConnectionString { get; set; } = null!;
	public string DatabaseName { get; set; } = null!;
}
namespace TarasZoukClasses.Data.Context
{
    using MongoDB.Driver;

    public interface IMongoDbContext
    {
        IMongoCollection<TEntity> GetCollection<TEntity>(string name);
    }
}

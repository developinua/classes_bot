namespace TarasZoukClasses.Data.Context
{
    using System;
    using System.Threading.Tasks;
    using MongoDB.Driver;

    public interface IMongoDbContext : IDisposable
    {
        IMongoCollection<TEntity> GetCollection<TEntity>(string name);

        void AddCommand(Func<Task> func);

        Task<int> SaveChanges();
    }
}

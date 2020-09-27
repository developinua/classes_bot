namespace TarasZoukClasses.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Models.MongoDb;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public abstract class MongoDbRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseMongoDbModel
    {
        private readonly IMongoDbContext _mongoContext;

        private IMongoCollection<TEntity> _dbCollection;

        protected MongoDbRepository(IMongoDbContext context)
        {
            _mongoContext = context;
            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public async Task Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(typeof(TEntity).Name + " object is null.");
            }

            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
            await _dbCollection.InsertOneAsync(entity);
        }

        public async Task Delete(int id)
        {
            //ex. 5dc1039a1521eaa36835e541

            var objectId = new ObjectId(id.ToString());
            await _dbCollection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", objectId));
        }

        public virtual async Task Update(TEntity entity)
        {
            //return Task.CompletedTask;
            await _dbCollection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", entity.Id), entity);
        }

        public async Task<TEntity> Get(int id)
        {
            //ex. 5dc1039a1521eaa36835e541

            var objectId = new ObjectId(id.ToString());

            var filter = Builders<TEntity>.Filter.Eq("_id", objectId);

            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);

            return await _dbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await _dbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
            return await all.ToListAsync();
        }
    }
}

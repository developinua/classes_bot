namespace TarasZoukClasses.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Models;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public abstract class MongoDbRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseMongoDbModel
    {
        protected readonly IMongoDbContext MongoContext;

        protected IMongoCollection<TEntity> DbCollection;

        protected MongoDbRepository(IMongoDbContext context)
        {
            MongoContext = context;
            DbCollection = MongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public async Task Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(typeof(TEntity).Name + " object is null.");
            }

            DbCollection = MongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
            await DbCollection.InsertOneAsync(entity);
        }

        public async Task Delete(int id)
        {
            //ex. 5dc1039a1521eaa36835e541

            var objectId = new ObjectId(id.ToString());
            await DbCollection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", objectId));
        }

        public virtual async Task Update(TEntity entity)
        {
            //return Task.CompletedTask;
            await DbCollection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", entity.Id), entity);
        }

        public async Task<TEntity> Get(int id)
        {
            //ex. 5dc1039a1521eaa36835e541

            var objectId = new ObjectId(id.ToString());

            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);

            DbCollection = MongoContext.GetCollection<TEntity>(typeof(TEntity).Name);

            return await DbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await DbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
            return await all.ToListAsync();
        }
    }
}

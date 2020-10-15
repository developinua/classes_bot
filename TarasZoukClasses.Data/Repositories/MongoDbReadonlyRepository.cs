namespace TarasZoukClasses.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Context;
    using Models.Base;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class MongoDbReadonlyRepository<TEntity> : IGenericReadonlyRepository<TEntity> where TEntity : class, IDocument
    {
        #region Properties: Private

        protected readonly IMongoCollection<TEntity> DbCollection;

        #endregion

        #region Constructor

        protected MongoDbReadonlyRepository(IMongoDbContext context)
        {
            DbCollection = context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        #endregion

        #region Filter

        public IQueryable<TEntity> AsQueryable()
        {
            return DbCollection.AsQueryable();
        }

        public async Task<IEnumerable<TEntity>> FilterBy(
            Expression<Func<TEntity, bool>> filterExpression)
        {
            return (await DbCollection.FindAsync(filterExpression)).ToEnumerable();
        }

        public IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TProjected>> projectionExpression)
        {
            return DbCollection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        #endregion

        #region Get

        public async Task<TEntity> GetAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
            var filteredDocument = await DbCollection.FindAsync(filter);

            return await filteredDocument.SingleOrDefaultAsync();
        }

        public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await DbCollection.Find(filterExpression).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var allDocs = await DbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
            return await allDocs.ToListAsync();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

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

    public abstract class MongoDbRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IDocument
    {
        #region Properties: Private

        protected readonly IMongoCollection<TEntity> DbCollection;

        #endregion

        #region Constructor

        protected MongoDbRepository(IMongoDbContext context)
        {
            DbCollection = context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        #endregion

        #region Methods: Public

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

        #region Insert

        public async Task InsertAsync(TEntity document)
        {
            await DbCollection.InsertOneAsync(document);
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> documents)
        {
            await DbCollection.InsertManyAsync(documents);
        }

        #endregion

        #region Update

        public async Task<TEntity> ReplaceAsync(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            return await DbCollection.FindOneAndReplaceAsync(filter, document);
        }

        #endregion

        #region Delete

        public async Task<TEntity> DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);

            return await DbCollection.FindOneAndDeleteAsync(filter);
        }

        public async Task<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await DbCollection.FindOneAndDeleteAsync(filterExpression);
        }

        public async Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await DbCollection.DeleteManyAsync(filterExpression);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion
    }
}

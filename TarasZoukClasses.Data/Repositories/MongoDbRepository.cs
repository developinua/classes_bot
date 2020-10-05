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

        public TEntity Get(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
            var filteredDocument = DbCollection.Find(filter);

            return filteredDocument.SingleOrDefault();
        }

        public async Task<TEntity> GetAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
            var filteredDocument = await DbCollection.FindAsync(filter);

            return await filteredDocument.SingleOrDefaultAsync();
        }

        public TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression)
        {
            return DbCollection.Find(filterExpression).FirstOrDefault();
        }

        public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await DbCollection.Find(filterExpression).FirstOrDefaultAsync();
        }

        public IEnumerable<TEntity> GetAll()
        {
            var allDocs = DbCollection.Find(Builders<TEntity>.Filter.Empty);
            return allDocs.ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var allDocs = await DbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
            return await allDocs.ToListAsync();
        }

        #endregion

        #region Insert

        public void Insert(TEntity document)
        {
            DbCollection.InsertOne(document);
        }

        public async Task InsertAsync(TEntity document)
        {
            await DbCollection.InsertOneAsync(document);
        }

        public void InsertMany(IEnumerable<TEntity> documents)
        {
            DbCollection.InsertMany(documents);
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> documents)
        {
            await DbCollection.InsertManyAsync(documents);
        }

        #endregion

        #region Update

        public TEntity Replace(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            return DbCollection.FindOneAndReplace(filter, document);
        }

        public async Task<TEntity> ReplaceAsync(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            return await DbCollection.FindOneAndReplaceAsync(filter, document);
        }

        #endregion

        #region Delete

        public TEntity Delete(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);

            return DbCollection.FindOneAndDelete(filter);
        }

        public TEntity Delete(Expression<Func<TEntity, bool>> filterExpression)
        {
            return DbCollection.FindOneAndDelete(filterExpression);
        }

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

        public DeleteResult DeleteMany(Expression<Func<TEntity, bool>> filterExpression)
        {
            return DbCollection.DeleteMany(filterExpression);
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

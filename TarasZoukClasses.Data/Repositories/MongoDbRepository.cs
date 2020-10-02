namespace TarasZoukClasses.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Context;
    using Models.MongoDb;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public abstract class MongoDbRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IDocument
    {
        #region Properties: Private

        private readonly IMongoCollection<TEntity> _dbCollection;

        #endregion

        #region Constructor

        protected MongoDbRepository(IMongoDbContext context)
        {
            _dbCollection = context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        #endregion

        #region Methods: Public

        #region Filter

        public IQueryable<TEntity> AsQueryable()
        {
            return _dbCollection.AsQueryable();
        }

        public async Task<IEnumerable<TEntity>> FilterBy(
            Expression<Func<TEntity, bool>> filterExpression)
        {
            return (await _dbCollection.FindAsync(filterExpression)).ToEnumerable();
        }

        public IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TProjected>> projectionExpression)
        {
            return _dbCollection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        #endregion

        #region Get

        public TEntity Get(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
            var filteredDocument = _dbCollection.Find(filter);

            return filteredDocument.SingleOrDefault();
        }

        public async Task<TEntity> GetAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
            var filteredDocument = await _dbCollection.FindAsync(filter);

            return await filteredDocument.SingleOrDefaultAsync();
        }

        public TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _dbCollection.Find(filterExpression).FirstOrDefault();
        }

        public async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await _dbCollection.Find(filterExpression).FirstOrDefaultAsync();
        }

        public IEnumerable<TEntity> GetAll()
        {
            var allDocs = _dbCollection.Find(Builders<TEntity>.Filter.Empty);
            return allDocs.ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var allDocs = await _dbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
            return await allDocs.ToListAsync();
        }

        #endregion

        #region Insert

        public void Insert(TEntity document)
        {
            _dbCollection.InsertOne(document);
        }

        public async Task InsertAsync(TEntity document)
        {
            await _dbCollection.InsertOneAsync(document);
        }

        public void InsertMany(IEnumerable<TEntity> documents)
        {
            _dbCollection.InsertMany(documents);
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> documents)
        {
            await _dbCollection.InsertManyAsync(documents);
        }

        #endregion

        #region Update

        public TEntity Replace(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            return _dbCollection.FindOneAndReplace(filter, document);
        }

        public async Task<TEntity> ReplaceAsync(TEntity document)
        {
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
            return await _dbCollection.FindOneAndReplaceAsync(filter, document);
        }

        #endregion

        #region Delete

        public TEntity Delete(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);

            return _dbCollection.FindOneAndDelete(filter);
        }

        public TEntity Delete(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _dbCollection.FindOneAndDelete(filterExpression);
        }

        public async Task<TEntity> DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);

            return await _dbCollection.FindOneAndDeleteAsync(filter);
        }

        public async Task<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await _dbCollection.FindOneAndDeleteAsync(filterExpression);
        }

        public DeleteResult DeleteMany(Expression<Func<TEntity, bool>> filterExpression)
        {
            return _dbCollection.DeleteMany(filterExpression);
        }

        public async Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression)
        {
            return await _dbCollection.DeleteManyAsync(filterExpression);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Classes.Domain.Repositories.Base;

public abstract class MongoDbRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class, IDocument
{
    protected readonly IMongoCollection<TEntity> DbCollection;

    protected MongoDbRepository(IMongoDbContext context) =>
        DbCollection = context.GetCollection<TEntity>(typeof(TEntity).Name);

    public IQueryable<TEntity> AsQueryable() => 
        DbCollection.AsQueryable();

    public async Task<IEnumerable<TEntity>> FilterBy(
        Expression<Func<TEntity, bool>> filterExpression) =>
        (await DbCollection.FindAsync(filterExpression)).ToEnumerable();

    public IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TProjected>> projectionExpression) =>
        DbCollection
            .Find(filterExpression)
            .Project(projectionExpression)
            .ToEnumerable();

    public async Task<TEntity> GetAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
        var filteredDocument = await DbCollection.FindAsync(filter);

        return await filteredDocument.SingleOrDefaultAsync();
    }

    public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression) => 
        await DbCollection.Find(filterExpression).FirstOrDefaultAsync();

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var allDocs = await DbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
        return await allDocs.ToListAsync();
    }

    public async Task InsertAsync(TEntity document) =>
        await DbCollection.InsertOneAsync(document);

    public async Task InsertManyAsync(IEnumerable<TEntity> documents) =>
        await DbCollection.InsertManyAsync(documents);

    public async Task<TEntity> ReplaceAsync(TEntity document)
    {
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, document.Id);
        return await DbCollection.FindOneAndReplaceAsync(filter, document);
    }

    public async Task<TEntity> DeleteAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);

        return await DbCollection.FindOneAndDeleteAsync(filter);
    }

    public async Task<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression) => 
        await DbCollection.FindOneAndDeleteAsync(filterExpression);

    public async Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression) => 
        await DbCollection.DeleteManyAsync(filterExpression);

    public void Dispose() => GC.SuppressFinalize(this);
}
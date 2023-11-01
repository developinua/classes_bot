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

public class MongoDbReadonlyRepository<TEntity> : IGenericReadonlyRepository<TEntity>
    where TEntity : class, IDocument
{
    private readonly IMongoCollection<TEntity> _dbCollection;

    protected MongoDbReadonlyRepository(IMongoDbContext context) =>
        _dbCollection = context.GetCollection<TEntity>(typeof(TEntity).Name);

    public IQueryable<TEntity> AsQueryable() => _dbCollection.AsQueryable();

    public async Task<IEnumerable<TEntity>> FilterBy(
        Expression<Func<TEntity, bool>> filterExpression) =>
        (await _dbCollection.FindAsync(filterExpression)).ToEnumerable();

    public IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TProjected>> projectionExpression) =>
        _dbCollection
            .Find(filterExpression)
            .Project(projectionExpression)
            .ToEnumerable();

    public async Task<TEntity> GetAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TEntity>.Filter.Eq(doc => doc.Id, objectId);
        var filteredDocument = await _dbCollection.FindAsync(filter);

        return await filteredDocument.SingleOrDefaultAsync();
    }

    public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression) => 
        await _dbCollection.Find(filterExpression).FirstOrDefaultAsync();

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var allDocs = await _dbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
        return await allDocs.ToListAsync();
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
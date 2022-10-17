using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Classes.Domain.Repositories.Base;

public interface IGenericRepository<TEntity> : IDisposable
    where TEntity : class
{
    IQueryable<TEntity> AsQueryable();
    Task<IEnumerable<TEntity>> FilterBy(
        Expression<Func<TEntity, bool>> filterExpression);
    IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, TProjected>> projectionExpression);

    Task<TEntity> GetAsync(string id);
    Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<IEnumerable<TEntity>> GetAllAsync();

    Task InsertAsync(TEntity document);
    Task InsertManyAsync(IEnumerable<TEntity> documents);
    Task<TEntity> ReplaceAsync(TEntity document);

    Task<TEntity> DeleteAsync(string id);
    Task<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression);
    Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression);
}
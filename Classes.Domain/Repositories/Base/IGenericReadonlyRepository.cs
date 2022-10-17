using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Classes.Domain.Repositories.Base;

public interface IGenericReadonlyRepository<TEntity> : IDisposable where TEntity : class
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
}
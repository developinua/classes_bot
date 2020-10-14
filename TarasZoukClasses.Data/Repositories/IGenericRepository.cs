namespace TarasZoukClasses.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MongoDB.Driver;

    public interface IGenericRepository<TEntity> : IDisposable
        where TEntity : class
    {
        #region Filter

        IQueryable<TEntity> AsQueryable();

        Task<IEnumerable<TEntity>> FilterBy(
            Expression<Func<TEntity, bool>> filterExpression);

        IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TProjected>> projectionExpression);

        #endregion

        #region Get

        Task<TEntity> GetAsync(string id);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression);

        Task<IEnumerable<TEntity>> GetAllAsync();

        #endregion

        #region Insert

        Task InsertAsync(TEntity document);

        Task InsertManyAsync(IEnumerable<TEntity> documents);

        #endregion

        #region Update

        Task<TEntity> ReplaceAsync(TEntity document);

        #endregion

        #region Delete

        Task<TEntity> DeleteAsync(string id);

        Task<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression);

        Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression);

        #endregion
    }
}

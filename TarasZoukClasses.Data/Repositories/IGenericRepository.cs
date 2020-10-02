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

        TEntity Get(string id);

        Task<TEntity> GetAsync(string id);

        TEntity FindOne(Expression<Func<TEntity, bool>> filterExpression);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filterExpression);

        IEnumerable<TEntity> GetAll();

        Task<IEnumerable<TEntity>> GetAllAsync();

        #endregion

        #region Insert

        void Insert(TEntity document);

        Task InsertAsync(TEntity document);

        void InsertMany(IEnumerable<TEntity> documents);

        Task InsertManyAsync(IEnumerable<TEntity> documents);

        #endregion

        #region Update

        TEntity Replace(TEntity document);

        Task<TEntity> ReplaceAsync(TEntity document);

        #endregion

        #region Delete

        TEntity Delete(string id);

        TEntity Delete(Expression<Func<TEntity, bool>> filterExpression);

        Task<TEntity> DeleteAsync(string id);

        Task<TEntity> DeleteAsync(Expression<Func<TEntity, bool>> filterExpression);

        DeleteResult DeleteMany(Expression<Func<TEntity, bool>> filterExpression);

        Task<DeleteResult> DeleteManyAsync(Expression<Func<TEntity, bool>> filterExpression);

        #endregion
    }
}

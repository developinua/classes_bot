namespace TarasZoukClasses.Data.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Get(int id);

        Task<IEnumerable<TEntity>> GetAll();

        Task Add(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(int id);
    }
}

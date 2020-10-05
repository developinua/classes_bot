namespace TarasZoukClasses.Domain.Service.CultureService
{
    using System.Threading.Tasks;
    using Data.Models;
    using Data.Repositories;

    public interface ICultureRepository : IGenericRepository<Culture>
    {
        Task<Culture> GetCultureByCodeAsync(string languageCode);
    }
}

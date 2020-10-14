namespace TarasZoukClasses.Domain.Service.ZoukUserAdditionalInformationService
{
    using Data.Context;
    using Data.Models;
    using Data.Repositories;

    public class ZoukUserAdditionalInformationMongoDbRepository : MongoDbRepository<ZoukUserAdditionalInformation>, IZoukUserAdditionalInformationRepository
    {
        public ZoukUserAdditionalInformationMongoDbRepository(IMongoDbContext context) : base(context) { }
    }
}

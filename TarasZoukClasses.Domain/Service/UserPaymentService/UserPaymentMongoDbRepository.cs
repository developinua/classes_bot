namespace TarasZoukClasses.Domain.Service.UserPaymentService
{
    using Data.Context;
    using Data.Models;
    using Data.Repositories;

    public class UserPaymentMongoDbRepository : MongoDbRepository<UserPayment>, IUserPaymentRepository
    {
        public UserPaymentMongoDbRepository(IMongoDbContext context) : base(context) {}
    }
}

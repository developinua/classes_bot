namespace TarasZoukClasses.Domain.Service.PaymentService
{
    using Data.Context;
    using Data.Repositories;
    using TarasZoukClasses.Data.Models;

    public class PaymentMongoDbRepository : MongoDbRepository<Payment>, IPaymentRepository
    {
        public PaymentMongoDbRepository(IMongoDbContext context) : base(context) { }
    }
}

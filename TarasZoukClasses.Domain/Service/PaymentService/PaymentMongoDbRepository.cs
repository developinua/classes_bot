namespace TarasZoukClasses.Domain.Service.PaymentService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TarasZoukClasses.Data.Models;

    public class PaymentMongoDbRepository : IPaymentRepository
    {

        public Task<Payment> Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Payment>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public Task Add(Payment entity)
        {
            throw new System.NotImplementedException();
        }

        public Task Update(Payment entity)
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}

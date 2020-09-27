namespace TarasZoukClasses.Domain.Service.PaymentService
{
    using BaseService;
    using Data.Models;
    using Data.Repositories;

    public interface IPaymentRepository : IGenericRepository<Payment> {}
}

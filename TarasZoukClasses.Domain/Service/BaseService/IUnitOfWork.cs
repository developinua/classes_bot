namespace TarasZoukClasses.Domain.Service.BaseService
{
    using System;
    using System.Threading.Tasks;
    using CommandService;
    using PaymentService;
    using UserPaymentService;
    using UserService;

    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; set; }

        IPaymentRepository Payments { get; set; }

        IUserPaymentRepository UsersPayments { get; set; }

        ICommandRepository Commands { get; set; }

        Task<int> SaveChanges();
    }
}

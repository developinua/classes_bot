namespace TarasZoukClasses.Domain.Service.BaseService
{
    using System;
    using System.Threading.Tasks;
    using CommandService;
    using CultureService;
    using PaymentService;
    using UserPaymentService;
    using UserService;

    public interface IUnitOfWork : IDisposable
    {
        IZoukUserRepository Users { get; set; }

        IPaymentRepository Payments { get; set; }

        IUserPaymentRepository UsersPayments { get; set; }

        ICultureRepository Cultures { get; set; }

        ICommandRepository Commands { get; set; }

        Task<int> SaveChanges();
    }
}

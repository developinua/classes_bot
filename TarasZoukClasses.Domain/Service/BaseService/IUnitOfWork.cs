namespace TarasZoukClasses.Domain.Service.BaseService
{
    using System;
    using CommandService;
    using PaymentService;
    using UserService;

    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; set; }

        IPaymentRepository Payments { get; set; }

        ICommandRepository Commands { get; set; }

        void Commit();

        void Rollback();
    }
}

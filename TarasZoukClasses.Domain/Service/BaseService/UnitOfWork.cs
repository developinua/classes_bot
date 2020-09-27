namespace TarasZoukClasses.Domain.Service.BaseService
{
    using CommandService;
    using PaymentService;
    using UserService;

    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository Users { get; set; }

        public IPaymentRepository Payments { get; set; }

        public ICommandRepository Commands { get; set; }

        public void Commit()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Rollback()
        {
            throw new System.NotImplementedException();
        }
    }
}

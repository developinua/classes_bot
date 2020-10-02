namespace TarasZoukClasses.Domain.Service.BaseService
{
    using System.Threading.Tasks;
    using CommandService;
    using Data.Context;
    using PaymentService;
    using UserPaymentService;
    using UserService;

    public class UnitOfWork : IUnitOfWork
    {
        private IMongoDbContext Context { get; }

        public IUserRepository Users { get; set; }

        public IPaymentRepository Payments { get; set; }

        public IUserPaymentRepository UsersPayments { get; set; }

        public ICommandRepository Commands { get; set; }

        public UnitOfWork(IMongoDbContext context)
        {
            Context = context;
        }

        public async Task<int> SaveChanges()
        {
            return await Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}

namespace TarasZoukClasses.Domain.Service.BaseService
{
    using System.Threading.Tasks;
    using CommandService;
    using CultureService;
    using Data.Context;
    using PaymentService;
    using UserPaymentService;
    using UserService;

    public class UnitOfWork : IUnitOfWork
    {
        private IMongoDbContext Context { get; }

        public IZoukUserRepository Users { get; set; }

        public IPaymentRepository Payments { get; set; }

        public IUserPaymentRepository UsersPayments { get; set; }

        public ICultureRepository Cultures { get; set; }

        public ICommandRepository Commands { get; set; }

        public UnitOfWork(IMongoDbContext context, IZoukUserRepository userRepository,
            IPaymentRepository paymentRepository, IUserPaymentRepository userPaymentRepository,
            ICommandRepository commandRepository, ICultureRepository cultures)
        {
            Context = context;
            Users = userRepository;
            Payments = paymentRepository;
            UsersPayments = userPaymentRepository;
            Commands = commandRepository;
            Cultures = cultures;
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

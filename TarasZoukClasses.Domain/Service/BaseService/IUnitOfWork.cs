namespace TarasZoukClasses.Domain.Service.BaseService
{
    using System;
    using System.Threading.Tasks;
    using CommandService;
    using CultureService;
    using PaymentService;
    using SubscriptionService;
    using ZoukUserAdditionalInformationService;
    using ZoukUserPaymentService;
    using ZoukUserService;
    using ZoukUserSubscription;

    public interface IUnitOfWork : IDisposable
    {
        IZoukUserRepository ZoukUsers { get; set; }

        IZoukUserAdditionalInformationRepository ZoukUsersAdditionalInformation { get; set; }

        IPaymentRepository Payments { get; set; }

        IZoukUserPaymentRepository ZoukUsersPayments { get; set; }

        ISubscriptionRepository Subscriptions { get; set; }

        IZoukUserSubscriptionRepository ZoukUsersSubscriptions { get; set; }

        ICultureRepository Cultures { get; set; }

        ICommandRepository Commands { get; set; }

        Task<int> SaveChanges();
    }
}

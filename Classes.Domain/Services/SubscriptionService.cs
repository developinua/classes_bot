using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Classes.Data.Repositories;
using ResultNet;

namespace Classes.Domain.Services;

public interface ISubscriptionService
{
    Task<Result<Subscription?>> GetActiveSubscriptionByTypeAndPeriodAsync(
        SubscriptionType subscriptionGroup,
        SubscriptionPeriod subscriptionPeriod);
}

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository) =>
        _subscriptionRepository = subscriptionRepository;

    public async Task<Result<Subscription?>> GetActiveSubscriptionByTypeAndPeriodAsync(
        SubscriptionType subscriptionGroup,
        SubscriptionPeriod subscriptionPeriod)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionByTypeAndPeriodAsync(
            subscriptionGroup,
            subscriptionPeriod);
        return Result.Success(subscription);
    }
}
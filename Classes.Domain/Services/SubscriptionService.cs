using System.Collections.Generic;
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

    Task<Result> Add(List<Subscription> subscriptions);
    Task<Result> RemoveActiveSubscriptions();
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

    public async Task<Result> Add(List<Subscription> subscriptions)
    {
        await _subscriptionRepository.Add(subscriptions);
        return Result.Success();
    }

    public async Task<Result> RemoveActiveSubscriptions()
    {
        await _subscriptionRepository.RemoveActiveSubscriptions();
        return Result.Success();
    }
}
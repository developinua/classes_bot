using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using ResultNet;

namespace Classes.Application.Services;

public interface ISubscriptionService
{
    Task<Result<Subscription?>> GetSubscriptionByTypeAndPeriod(SubscriptionType type, SubscriptionPeriod period);
}

public class SubscriptionService(
        ISubscriptionRepository subscriptionRepository)
    : ISubscriptionService
{
    public async Task<Result<Subscription?>> GetSubscriptionByTypeAndPeriod(
        SubscriptionType type,
        SubscriptionPeriod period)
    {
        var subscription = await subscriptionRepository.GetActiveByTypeAndPeriod(type, period);
        return subscription.IsFailure() ? subscription : Result.Success(subscription.Data);
    }
}
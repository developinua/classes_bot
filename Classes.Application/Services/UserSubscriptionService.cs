using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using Microsoft.Extensions.Logging;
using ResultNet;

namespace Classes.Application.Services;

public interface IUserSubscriptionService
{
    Task<Result<UserSubscription?>> GetById(long id);
    Task<Result> CheckinOnClass(UserSubscription userSubscription);
}

public class UserSubscriptionService(
        IUserSubscriptionRepository userSubscriptionRepository,
        ILogger<UserService> logger)
    : IUserSubscriptionService
{
    public async Task<Result<UserSubscription?>> GetById(long id) =>
        await userSubscriptionRepository.GetById(id);

    public async Task<Result> CheckinOnClass(UserSubscription userSubscription)
    {
        userSubscription.RemainingClasses--;
        return await userSubscriptionRepository.Update(userSubscription);
    }
}
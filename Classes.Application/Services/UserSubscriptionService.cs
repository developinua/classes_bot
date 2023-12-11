using System.Collections.Generic;
using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using ResultNet;

namespace Classes.Application.Services;

public interface IUserSubscriptionService
{
    Task<Result<UserSubscription?>> GetById(long id);
    Result<bool> CanCheckinOnClass(UserSubscription userSubscription);
    Task<Result> CheckinOnClass(UserSubscription userSubscription);
    Task<Result<IReadOnlyCollection<UserSubscription>>> GetAllActiveWithRemainingClasses(string username);
}

public class UserSubscriptionService(IUserSubscriptionRepository userSubscriptionRepository) : IUserSubscriptionService
{
    public async Task<Result<UserSubscription?>> GetById(long id) =>
        await userSubscriptionRepository.GetById(id);

    public Result<bool> CanCheckinOnClass(UserSubscription userSubscription) =>
        userSubscription.RemainingClasses == 0
            ? Result.Failure<bool>().WithMessage("No available classes.")
            : Result.Success(true);

    public async Task<Result> CheckinOnClass(UserSubscription userSubscription)
    {
        if (userSubscription.RemainingClasses == 0)
            return Result.Failure().WithMessage("No available classes.");
        
        userSubscription.RemainingClasses--;
        return await userSubscriptionRepository.Update(userSubscription);
    }

    public async Task<Result<IReadOnlyCollection<UserSubscription>>> GetAllActiveWithRemainingClasses(string username)
    {
        return await userSubscriptionRepository.GetAllActiveWithRemainingClasses(username);
    }
}
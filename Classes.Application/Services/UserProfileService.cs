using System.Threading.Tasks;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Services;

public interface IUserProfileService
{
    UserProfile CreateUserProfile(CallbackQuery callbackQuery, Culture culture);
    Task<Result> UpdateUserProfile(UserProfile userProfile);
}

public class UserProfileService(IUserProfileRepository userProfileRepository) : IUserProfileService
{
    public UserProfile CreateUserProfile(CallbackQuery callbackQuery, Culture culture)
    {
        return new UserProfile
        {
            ChatId = callbackQuery.From.Id,
            FirstName = callbackQuery.From.FirstName,
            LastName = callbackQuery.From.LastName,
            IsPremium = callbackQuery.From.IsPremium.GetValueOrDefault(),
            IsBot = callbackQuery.From.IsBot,
            Culture = culture
        };
    }

    public async Task<Result> UpdateUserProfile(UserProfile userProfile)
    {
        var userProfileDb = await userProfileRepository.GetUserProfileByChatId(userProfile.ChatId);
        return userProfileDb is null
            ? await userProfileRepository.CreateAsync(userProfile)
            : await userProfileRepository.UpdateAsync(userProfile);
    }
}
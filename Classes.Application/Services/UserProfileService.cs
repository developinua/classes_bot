using System.Threading.Tasks;
using AutoMapper;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Services;

public interface IUserProfileService
{
    UserProfile GetUserProfileFromMessage(Message message, Culture culture);
    UserProfile GetUserProfileFromCallback(CallbackQuery callback, Culture culture);
    Task<Result> UpdateUserProfile(UserProfile userProfile);
    Task<Result> Create(UserProfile userProfile);
}

public class UserProfileService(IUserProfileRepository userProfileRepository, IMapper mapper) : IUserProfileService
{
    public UserProfile GetUserProfileFromMessage(Message message, Culture culture)
    {
        return new UserProfile
        {
            ChatId = message.From!.Id,
            FirstName = message.From.FirstName,
            LastName = message.From.LastName,
            IsPremium = message.From.IsPremium.GetValueOrDefault(),
            IsBot = message.From.IsBot,
            Culture = culture
        };
    }

    public UserProfile GetUserProfileFromCallback(CallbackQuery callback, Culture culture)
    {
        return new UserProfile
        {
            ChatId = callback.From.Id,
            FirstName = callback.From.FirstName,
            LastName = callback.From.LastName,
            IsPremium = callback.From.IsPremium.GetValueOrDefault(),
            IsBot = callback.From.IsBot,
            Culture = culture
        };
    }

    public async Task<Result> UpdateUserProfile(UserProfile userProfile)
    {
        var userProfileDb = await userProfileRepository.GetUserProfileByChatId(userProfile.ChatId);

        if (userProfileDb.Data is null)
            return await userProfileRepository.CreateAsync(userProfile);
        
        var updatedUserProfile = mapper.Map(userProfileDb.Data, userProfile);
        
        return await userProfileRepository.UpdateAsync(updatedUserProfile);
    }

    public async Task<Result> Create(UserProfile userProfile) =>
        await userProfileRepository.CreateAsync(userProfile);
}
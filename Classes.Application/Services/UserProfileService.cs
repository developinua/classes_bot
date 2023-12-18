using System.Threading.Tasks;
using AutoMapper;
using Classes.Data.Repositories;
using Classes.Domain.Models;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Application.Services;

public interface IUserProfileService
{
    UserProfile CreateUserProfile(CallbackQuery callbackQuery, Culture culture);
    Task<Result> UpdateUserProfile(UserProfile userProfile);
    Task<Result> Create(UserProfile userProfile);
}

public class UserProfileService(IUserProfileRepository userProfileRepository, IMapper mapper) : IUserProfileService
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

        if (userProfileDb.Data is null)
            return await userProfileRepository.CreateAsync(userProfile);
        
        var updatedUserProfile = mapper.Map(userProfileDb.Data, userProfile);
        
        return await userProfileRepository.UpdateAsync(updatedUserProfile);
    }

    public async Task<Result> Create(UserProfile userProfile) =>
        await userProfileRepository.CreateAsync(userProfile);
}
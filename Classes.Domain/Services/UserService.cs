using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Domain.Repositories;
using Telegram.Bot.Types;
using User = Classes.Data.Models.User;

namespace Classes.Domain.Services;

public interface IUserService
{
    Task SaveUser(CallbackQuery callbackQuery, string callbackQueryPattern);
}

public class UserService : IUserService
{
    private readonly ICultureRepository _cultureRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public UserService(
        ICultureRepository cultureRepository,
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository)
    {
        _cultureRepository = cultureRepository;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task SaveUser(CallbackQuery callbackQuery, string callbackQueryPattern)
    {
        var cultureName = GetCultureNameFromCallbackQuery(callbackQuery.Data, callbackQueryPattern);
        var culture = await _cultureRepository.GetCultureByCodeAsync(cultureName) ?? new();
        var nickName = callbackQuery.From.Username ?? callbackQuery.From.Id.ToString();
        var userExists = await _userRepository.GetUserByNickname(nickName) is not null;
        var userProfile = new UserProfile
        {
            ChatId = callbackQuery.From.Id,
            FirstName = callbackQuery.From.FirstName,
            LastName = callbackQuery.From.LastName,
            IsPremium = callbackQuery.From.IsPremium.GetValueOrDefault(),
            IsBot = callbackQuery.From.IsBot,
            Culture = culture
        };

        if (userExists)
            await UpdateUserProfile(userProfile);
        else
            await CreateUser(nickName, userProfile);
    }
    
    private async Task CreateUser(string nickName, UserProfile userProfile)
    {
        await _userProfileRepository.CreateAsync(userProfile);
        await _userRepository.CreateAsync(new User
        {
            NickName = nickName,
            UserProfile = userProfile
        });
    }
    
    private async Task UpdateUserProfile(UserProfile userProfile)
    {
        var userProfileDb = await _userProfileRepository.GetUserProfileByChatId(userProfile.ChatId);

        if (userProfileDb is null)
            await _userProfileRepository.CreateAsync(userProfile);
        else
            await _userProfileRepository.UpdateAsync(userProfile);
    }

    private static string GetCultureNameFromCallbackQuery(string? callbackQueryData, string callbackQueryPattern)
    {
        var cultureName = string.Empty;
        var cultureMatch = Regex.Match(callbackQueryData ?? "", callbackQueryPattern);

        if (cultureMatch.Success && cultureMatch.Groups["query"].Value.Equals("language"))
            cultureName = cultureMatch.Groups["data"].Value;

        if (string.IsNullOrEmpty(cultureName))
            throw new ArgumentException("Culture name can't be parsed.");

        return cultureName;
    }
}
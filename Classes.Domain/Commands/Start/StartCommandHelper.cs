using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Domain.Repositories;
using Telegram.Bot.Types;
using User = Classes.Data.Models.User;

namespace Classes.Domain.Commands.Start;

public class StartCommandHelper
{
    public static async Task SaveUser(IUnitOfWork services, CallbackQuery callbackQuery, string callbackQueryPattern)
    {
        var cultureName = GetCultureNameFromCallbackQuery(callbackQuery.Data, callbackQueryPattern);
        var culture = await services.Cultures.GetCultureByCodeAsync(cultureName);
        var dbUser = await services.Users.FindOneAsync(x =>
            x.NickName == callbackQuery.Message.Chat.Username);
        var userAdditionalInfo = new UserInformation
        {
            Culture = culture,
            ChatId = callbackQuery.From.Id,
            FirstName = callbackQuery.Message.Chat.FirstName,
            SecondName = callbackQuery.Message.Chat.LastName
        };

        if (dbUser != null)
            await UpdateUser(services, dbUser, userAdditionalInfo);
        else
            await CreateUser(services, callbackQuery.Message.Chat.Username, userAdditionalInfo);
    }

    private static async Task UpdateUser(IUnitOfWork services, User user, UserInformation userInformation)
    {
        var userInfoAlreadyStoredInDb = await services.UsersInformation.FindOneAsync(x =>
            x.FirstName == userInformation.FirstName
            && x.ChatId == userInformation.ChatId);

        if (userInfoAlreadyStoredInDb == null)
            await services.UsersInformation.InsertAsync(userInformation);
        else
            await services.UsersInformation.ReplaceAsync(userInformation);

        user.UserInformation = userInformation;
        await services.Users.ReplaceAsync(user);
    }

    private static async Task CreateUser(IUnitOfWork services, string username, UserInformation userInformation)
    {
        await services.UsersInformation.InsertAsync(userInformation);
        await services.Users.InsertAsync(new User
        {
            NickName = username,
            UserInformation = userInformation
        });
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
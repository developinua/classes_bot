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
        var dbUser = await services.ZoukUsers.FindOneAsync(x =>
            x.NickName == callbackQuery.Message.Chat.Username);
        var zoukUserAdditionalInfo = new UserInformation
        {
            Culture = culture,
            ChatId = callbackQuery.From.Id,
            FirstName = callbackQuery.Message.Chat.FirstName,
            SecondName = callbackQuery.Message.Chat.LastName
        };

        if (dbUser != null)
            await UpdateUser(services, dbUser, zoukUserAdditionalInfo);
        else
            await CreateUser(services, callbackQuery.Message.Chat.Username, zoukUserAdditionalInfo);
    }

    private static async Task UpdateUser(IUnitOfWork services, User zoukUser, UserInformation zoukUserAdditionalInfo)
    {
        var zoukUserInfoAlreadyStoredInDb = await services.ZoukUsersAdditionalInformation.FindOneAsync(x =>
            x.FirstName == zoukUserAdditionalInfo.FirstName
            && x.ChatId == zoukUserAdditionalInfo.ChatId);

        if (zoukUserInfoAlreadyStoredInDb == null)
            await services.ZoukUsersAdditionalInformation.InsertAsync(zoukUserAdditionalInfo);
        else
            await services.ZoukUsersAdditionalInformation.ReplaceAsync(zoukUserAdditionalInfo);

        zoukUser.UserInformation = zoukUserAdditionalInfo;
        await services.ZoukUsers.ReplaceAsync(zoukUser);
    }

    private static async Task CreateUser(IUnitOfWork services, string username, UserInformation zoukUserAdditionalInformation)
    {
        await services.ZoukUsersAdditionalInformation.InsertAsync(zoukUserAdditionalInformation);
        await services.ZoukUsers.InsertAsync(new User
        {
            NickName = username,
            UserInformation = zoukUserAdditionalInformation
        });
    }

    private static string GetCultureNameFromCallbackQuery(string callbackQueryData, string callbackQueryPattern)
    {
        var cultureName = string.Empty;
        var cultureMatch = Regex.Match(callbackQueryData, callbackQueryPattern);

        if (cultureMatch.Success && cultureMatch.Groups["query"].Value.Equals("language"))
            cultureName = cultureMatch.Groups["data"].Value;

        if (string.IsNullOrEmpty(cultureName))
            throw new ArgumentException("Culture name can't be parsed.");

        return cultureName;
    }
}
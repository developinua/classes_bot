using System;
using System.Text.RegularExpressions;
using Classes.Domain.Models.Enums;
using Microsoft.Extensions.Logging;

namespace Classes.Application.Services;

public interface ICallbackExtractorService
{
    string GetCultureNameFromCallbackQuery(string? callbackData, string callbackPattern);
    long GetUserSubscriptionIdFromCallback(string callbackData, string callbackPattern);
    SubscriptionType GetSubscriptionType(string callbackData);
    SubscriptionPeriod GetSubscriptionPeriod(string callbackData);
    SubscriptionsCallbackQueryType GetSubscriptionCallbackQueryType(string callbackData);
}

public class CallbackExtractorService(ILogger<CallbackExtractorService> logger) : ICallbackExtractorService
{
    public string GetCultureNameFromCallbackQuery(string? callbackData, string callbackPattern)
    {
        var cultureName = string.Empty;
        var cultureMatch = Regex.Match(callbackData ?? "", callbackPattern);

        if (cultureMatch.Success && cultureMatch.Groups["query"].Value.Equals("language"))
            cultureName = cultureMatch.Groups["data"].Value;

        if (string.IsNullOrEmpty(cultureName))
        {
            logger.LogWarning("Culture name can't be parsed.");
            return "en-US";
        }
        
        return cultureName;
    }
    
    public long GetUserSubscriptionIdFromCallback(string callbackData, string callbackPattern)
    {
        var userSubscriptionIdGroup = string.Empty;
        var userSubscriptionIdGroupMatch = Regex.Match(callbackData, callbackPattern);
        var userSubscriptionIdGroupMatchQuery = userSubscriptionIdGroupMatch.Groups["query"].Value;
        var userSubscriptionIdGroupMatchData = userSubscriptionIdGroupMatch.Groups["data"].Value;

        if (userSubscriptionIdGroupMatch.Success
            && userSubscriptionIdGroupMatchQuery.Equals("checkinUserSubscriptionId"))
            userSubscriptionIdGroup = userSubscriptionIdGroupMatchData;

        return long.Parse(userSubscriptionIdGroup);
    }
    
    public SubscriptionType GetSubscriptionType(string callbackData)
    {
        var subscriptionTypeString = string.Empty;
        var subscriptionTypeMatch = Regex.Match(callbackData, @"(?i)(?<query>subsGroup):(?<data>\w+)");
        var subscriptionTypeMatchQuery = subscriptionTypeMatch.Groups["query"].Value;
        var subscriptionTypeMatchData = subscriptionTypeMatch.Groups["data"].Value;

        if (subscriptionTypeMatch.Success && subscriptionTypeMatchQuery.Equals("subsGroup"))
            subscriptionTypeString = subscriptionTypeMatchData;

        Enum.TryParse(subscriptionTypeString, true, out SubscriptionType subscriptionType);

        return subscriptionType;
    }

    public SubscriptionPeriod GetSubscriptionPeriod(string callbackData)
    {
        var subscriptionPeriodString = string.Empty;
        var subscriptionPeriodMatch = Regex.Match(callbackData, @"(?i)(?<query>subsPeriod):(?<data>\w+)");
        var subscriptionPeriodMatchQuery = subscriptionPeriodMatch.Groups["query"].Value;
        var subscriptionPeriodMatchData = subscriptionPeriodMatch.Groups["data"].Value;

        if (subscriptionPeriodMatch.Success && subscriptionPeriodMatchQuery.Equals("subsPeriod"))
            subscriptionPeriodString = subscriptionPeriodMatchData;
        
        Enum.TryParse(subscriptionPeriodString, true, out SubscriptionPeriod subscriptionPeriod);

        return subscriptionPeriod;
    }
    
    public SubscriptionsCallbackQueryType GetSubscriptionCallbackQueryType(string callbackData) =>
        callbackData switch
        {
            _ when callbackData.Contains("subsGroup") && !callbackData.Contains("subsPeriod") =>
                SubscriptionsCallbackQueryType.Group,
            _ when callbackData.Contains("subsGroup") && callbackData.Contains("subsPeriod") =>
                SubscriptionsCallbackQueryType.Period,
            _ => SubscriptionsCallbackQueryType.None
        };
}
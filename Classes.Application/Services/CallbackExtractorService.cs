using System;
using System.Text.RegularExpressions;
using Classes.Domain.Models;
using Classes.Domain.Models.Enums;
using Microsoft.Extensions.Logging;

namespace Classes.Application.Services;

public interface ICallbackExtractorService
{
    string GetCultureNameFrom(string? callbackData, string callbackPattern);
    long GetUserSubscriptionId(string callbackData, string callbackPattern);
    SubscriptionType GetSubscriptionType(string callbackData, string callbackPattern);
}

public class CallbackExtractorService(ILogger<CallbackExtractorService> logger) : ICallbackExtractorService
{
    public string GetCultureNameFrom(string? callbackData, string callbackPattern)
    {
        var cultureName = string.Empty;
        var cultureMatch = Regex.Match(callbackData ?? "", callbackPattern);

        if (cultureMatch.Success && cultureMatch.Groups["query"].Value.Equals("language"))
            cultureName = cultureMatch.Groups["data"].Value;

        if (!string.IsNullOrEmpty(cultureName)) return cultureName;

        logger.LogWarning("Culture name can't be parsed.");
        return new Culture().Name;
    }
    
    public long GetUserSubscriptionId(string callbackData, string callbackPattern)
    {
        var userSubscriptionIdGroup = string.Empty;
        var userSubscriptionIdGroupMatch = Regex.Match(callbackData, callbackPattern);
        var userSubscriptionIdGroupMatchQuery = userSubscriptionIdGroupMatch.Groups["query"].Value;
        var userSubscriptionIdGroupMatchData = userSubscriptionIdGroupMatch.Groups["data"].Value;

        if (userSubscriptionIdGroupMatch.Success
            && userSubscriptionIdGroupMatchQuery.Equals("user-subscription-id"))
            userSubscriptionIdGroup = userSubscriptionIdGroupMatchData;

        return long.Parse(userSubscriptionIdGroup);
    }
    
    public SubscriptionType GetSubscriptionType(string callbackData, string callbackPattern)
    {
        var subscriptionTypeString = string.Empty;
        var subscriptionTypeMatch = Regex.Match(callbackData, callbackPattern);
        var subscriptionTypeMatchQuery = subscriptionTypeMatch.Groups["query"].Value;
        var subscriptionTypeMatchData = subscriptionTypeMatch.Groups["data"].Value;

        if (subscriptionTypeMatch.Success && subscriptionTypeMatchQuery.Equals("subscription"))
            subscriptionTypeString = subscriptionTypeMatchData;

        Enum.TryParse(subscriptionTypeString, true, out SubscriptionType subscriptionType);

        return subscriptionType;
    }
}
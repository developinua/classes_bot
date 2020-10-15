namespace TarasZoukClasses.Domain.Commands.MySubscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Data.Models;
    using Data.Models.Subscription;
    using MongoDB.Bson;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;
    using Utils;

    public static class MySubscriptionsCommandHelper
    {
        internal static async Task ParseSubscription(int chatId, CallbackQuery callbackQuery,
            TelegramBotClient client, IUnitOfWork services)
        {
            var subscriptionCallbackQueryType = GetSubscriptionCallbackQueryType(callbackQuery.Data);

            var temp = subscriptionCallbackQueryType switch
            {
                MySubscriptionsCallbackQueryType.Group => SendSubscriptionGroupTextMessage(chatId, client, callbackQuery.Data),
                MySubscriptionsCallbackQueryType.Period => await SendSubscriptionPeriodTextMessage(chatId, client, services, callbackQuery.Data),
                _ => throw new NotImplementedException("No such subscription callback query type was founded.")
            };

            if (temp != null)
                await temp.Invoke();
        }

        #region Callback Query Type

        private static MySubscriptionsCallbackQueryType GetSubscriptionCallbackQueryType(string callbackQueryData) => callbackQueryData switch
        {
            var s when s.Contains("subsgroup") && !s.Contains("subsperiod") => MySubscriptionsCallbackQueryType.Group,
            var s when s.Contains("subsgroup") && s.Contains("subsperiod") => MySubscriptionsCallbackQueryType.Period,
            _ => throw new InvalidDataContractException("Can't parse subscription subsgroup from user callback query data.")
        };

        #endregion

        #region Text Message

        private static string RenderZoukUserSubscriptionInformationText(ZoukUserSubscription zoukUserSubscription)
        {
            var priceText = zoukUserSubscription.Subscription.DiscountPercent == 0
                ? $"Price: {zoukUserSubscription.Subscription.GetSummaryPrice()}\n"
                : $"Price with discount: {zoukUserSubscription.Subscription.GetSummaryPrice()}\n";

            return $"Subscription: {zoukUserSubscription.Subscription.Name}\n" +
                   $"{priceText}" +
                   $"Description: {zoukUserSubscription.Subscription.Description}\n" +
                   $"SubscriptionType: {zoukUserSubscription.Subscription.Type}\n" +
                   $"Total Classes: {zoukUserSubscription.RemainingClassesCount}\n";
        }

        private static Func<Task<Message>> SendSubscriptionGroupTextMessage(int chatId, TelegramBotClient client, string callbackQueryData)
        {
            var replyKeyboardMarkup = RenderSubscriptionPeriods(callbackQueryData);
            return () => client.SendTextMessageAsync(chatId, "*Which subscription subsperiod do you prefer?\n*", ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
        }

        private static async Task<Func<Task<Message>>> SendSubscriptionPeriodTextMessage(int chatId, TelegramBotClient client, IUnitOfWork services, string callbackQueryData)
        {
            Enum.TryParse(GetSubscriptionGroupDataFromCallbackQuery(callbackQueryData), out SubscriptionType subsgroup);
            Enum.TryParse(GetSubscriptionPeriodDataFromCallbackQuery(callbackQueryData), out SubscriptionPeriod subsperiod);

            var subscription = await services.Subscriptions.FindOneAsync(x => x.IsActive
                && x.Type.Equals(subsgroup)
                && x.Period.Equals(subsperiod));

            if (subscription == null)
                return async () => await client.SendTextMessageAsync(chatId, "No available subscription was founded.\nPlease contact @nazikBro");

            var replyKeyboardMarkup = RenderBuySubscription(subscription.Id);

            await client.SendTextMessageAsync(chatId, $"*Price: {subscription.GetSummaryPrice()}\n*P.S. Please send your nickname in comment", ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
            return async () => await client.SendTextMessageAsync(chatId, "*After your subscription will be approved by teacher\nYou will be able to /checkin on classes.*", ParseMode.Markdown);
        }

        #endregion

        #region Data From Callback Query

        private static string GetSubscriptionGroupDataFromCallbackQuery(string callbackQueryData)
        {
            var subscriptionGroup = string.Empty;
            var subscriptionGroupMatch = Regex.Match(callbackQueryData, @"(?i)(?<query>subsgroup):(?<data>\w+)");
            var subscriptionGroupMatchQuery = subscriptionGroupMatch.Groups["query"].Value;
            var subscriptionGroupMatchData = subscriptionGroupMatch.Groups["data"].Value;

            if (subscriptionGroupMatch.Success && subscriptionGroupMatchQuery.Equals("subsgroup"))
                subscriptionGroup = subscriptionGroupMatchData;

            return subscriptionGroup;
        }

        private static string GetSubscriptionPeriodDataFromCallbackQuery(string callbackQueryData)
        {
            var subscriptionPeriod = string.Empty;
            var subscriptionPeriodMatch = Regex.Match(callbackQueryData, @"(?i)(?<query>subsperiod):(?<data>\w+)");
            var subscriptionPeriodMatchQuery = subscriptionPeriodMatch.Groups["query"].Value;
            var subscriptionPeriodMatchData = subscriptionPeriodMatch.Groups["data"].Value;

            if (subscriptionPeriodMatch.Success && subscriptionPeriodMatchQuery.Equals("subsperiod"))
                subscriptionPeriod = subscriptionPeriodMatchData;

            return subscriptionPeriod;
        }

        #endregion

        #region Information

        internal static async Task GetZoukUserSubscriptionInformation(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            var chatId = message.From.Id;
            var zoukUserSubscriptions = (await GetZoukUserSubscriptions(message.From.Username, services)).ToList();

            if (!zoukUserSubscriptions.Any())
            {
                await GetNewZoukUserSubscriptionInformation(client, chatId);
                return;
            }

            await GetExistingZoukUserSubscriptionInformation(client, zoukUserSubscriptions, chatId);
        }

        private static async Task<IEnumerable<ZoukUserSubscription>> GetZoukUserSubscriptions(string userName, IUnitOfWork services)
        {
            return await services.ZoukUsersSubscriptions.FilterBy(x =>
                x.ZoukUser.NickName == userName
                && x.RemainingClassesCount > 0
                && x.Subscription.IsActive);
        }

        private static async Task GetExistingZoukUserSubscriptionInformation(TelegramBotClient client, List<ZoukUserSubscription> zoukUserSubscriptions, int chatId)
        {
            var pluralEnding = zoukUserSubscriptions.Count > 1 ? "s" : "";
            await client.SendTextMessageAsync(chatId, $"*Your subscription{pluralEnding}:*", ParseMode.Markdown);

            foreach (var zoukUserSubscription in zoukUserSubscriptions)
            {
                var replyMessage = RenderZoukUserSubscriptionInformationText(zoukUserSubscription);
                await client.SendTextMessageAsync(chatId, replyMessage, ParseMode.Markdown);
            }

            // TODO: Change to add functionality for adding few subscriptions
            await client.SendTextMessageAsync(chatId, "*Do you want to /checkin class?*", ParseMode.Markdown);
        }

        private static async Task GetNewZoukUserSubscriptionInformation(TelegramBotClient client, int chatId)
        {
            const string responseMessage = "*Which subscription do you want choose?\n*";
            var replyKeyboardMarkup = RenderSubscriptionGroups();

            await client.SendTextMessageAsync(chatId, responseMessage, ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
        }

        #endregion

        #region Render

        private static InlineKeyboardMarkup RenderSubscriptionGroups()
        {
            return InlineKeyboardBuilder.Create()
                .AddButton("Novice subscription", "subsgroup:Novice")
                .NewLine()
                .AddButton("Medium subscription", "subsgroup:Medium")
                .NewLine()
                .AddButton("Lady Style subscription", "subsgroup:Lady")
                .NewLine()
                .AddButton("Novice and Medium subscription", "subsgroup:NoviceMedium")
                .NewLine()
                .AddButton("Novice and Lady Style subscription", "subsgroup:NoviceLady")
                .NewLine()
                .AddButton("Medium and Lady Style subscription", "subsgroup:MediumLady")
                .NewLine()
                .AddButton("Premium", "subsgroup:Premium")
                .Build();
        }

        private static InlineKeyboardMarkup RenderSubscriptionPeriods(string subscriptionGroupData)
        {
            var subscriptionPeriods = InlineKeyboardBuilder.Create();

            if (!subscriptionGroupData.Contains("Premium"))
                subscriptionPeriods.AddButton("Day", $"{subscriptionGroupData}?subsperiod:Day").NewLine();

            subscriptionPeriods.AddButton("Week", $"{subscriptionGroupData}?subsperiod:Week")
                .NewLine()
                .AddButton("Two Weeks", $"{subscriptionGroupData}?subsperiod:TwoWeeks")
                .NewLine()
                .AddButton("Month", $"{subscriptionGroupData}?subsperiod:Month")
                .NewLine()
                .AddButton("Three Months", $"{subscriptionGroupData}?subsperiod:ThreeMonths");

            return subscriptionPeriods.Build();
        }

        private static InlineKeyboardMarkup RenderBuySubscription(ObjectId id)
        {
            // TODO: Add replacement link
            return InlineKeyboardBuilder.Create()
                .AddUrlButton("Buy", $"paymentlink:{id}", "https://send.monobank.ua/3EVPSXNq4F")
                .Build();
        }

        #endregion
    }
}

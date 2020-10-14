namespace TarasZoukClasses.Domain.Commands
{
    using System;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Data.Models;
    using MongoDB.Bson;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;
    using Utils;

    public class SubscriptionCommand : ICommand
    {
        public string Name => @"/subscription";

        public string CallbackQueryPattern => @"(?i)(?<query>subscription)";

        public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

        public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

        public async Task Execute(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            await client.SendChatActionAsync(message.From.Id, ChatAction.Typing);
            await SubscriptionCommandHelper.GetZoukUserSubscriptionInformation(message, client, services);
        }

        public async Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services)
        {
            if (callbackQuery.Validate())
                throw new NotSupportedException();

            var chatId = callbackQuery.From.Id;
            await client.SendChatActionAsync(chatId, ChatAction.Typing);
            await SubscriptionCommandHelper.ParseSubscription(chatId, callbackQuery, client, services);
        }

        private static class SubscriptionCommandHelper
        {
            internal static async Task ParseSubscription(int chatId, CallbackQuery callbackQuery,
                TelegramBotClient client, IUnitOfWork services)
            {
                var subscriptionCallbackQueryType = GetSubscriptionCallbackQueryType(callbackQuery.Data);

                var temp = subscriptionCallbackQueryType switch
                {
                    SubscriptionCallbackQueryType.Group => GetSubscriptionGroupTextMessage(chatId, client, callbackQuery.Data),
                    SubscriptionCallbackQueryType.Period => await GetSubscriptionPeriodTextMessage(chatId, client, services, callbackQuery.Data),
                    _ => throw new NotImplementedException()
                };

                if (temp != null)
                    await temp.Invoke();
            }

            private static SubscriptionCallbackQueryType GetSubscriptionCallbackQueryType(string callbackQueryData) => callbackQueryData switch
            {
                var s when s.Contains("subscriptionGroup") && !s.Contains("subscriptionPeriod") => SubscriptionCallbackQueryType.Group,
                var s when s.Contains("subscriptionGroup") && s.Contains("subscriptionPeriod") => SubscriptionCallbackQueryType.Period,
                _ => throw new InvalidDataContractException("Can't parse subscription group from user callback query data.")
            };

            private static Func<Task<Message>> GetSubscriptionGroupTextMessage(int chatId, TelegramBotClient client, string callbackQueryData)
            {
                var replyKeyboardMarkup = RenderSubscriptionPeriods(callbackQueryData);
                return () => client.SendTextMessageAsync(chatId, "*Which subscription period do you prefer?\n*", ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
            }

            private static async Task<Func<Task<Message>>> GetSubscriptionPeriodTextMessage(int chatId, TelegramBotClient client, IUnitOfWork services, string callbackQueryData)
            {
                Enum.TryParse(GetSubscriptionGroupDataFromCallbackQuery(callbackQueryData), out SubscriptionType group);
                Enum.TryParse(GetSubscriptionPeriodDataFromCallbackQuery(callbackQueryData), out SubscriptionPeriod period);

                var subscription = await services.Subscriptions.FindOneAsync(x => x.IsActive
                    && x.Type.Equals(group)
                    && x.Period.Equals(period));

                if (subscription == null)
                    return async () => await client.SendTextMessageAsync(chatId, "No available subscription was founded.\nPlease contact @nazikBro");

                var replyKeyboardMarkup = RenderSubscription(subscription.Id);

                await client.SendTextMessageAsync(chatId, $"*Price: {subscription.Price}\n*P.S. Please send your nickname in comment", ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
                return async () => await client.SendTextMessageAsync(chatId, "*After your subscription will be approved by teacher\nYou will be able to check in on classes.*", ParseMode.Markdown);
            }

            private static string GetSubscriptionGroupDataFromCallbackQuery(string callbackQueryData)
            {
                var subscriptionGroup = string.Empty;
                var subscriptionGroupMatch = Regex.Match(callbackQueryData, @"(?i)(?<query>subscriptionGroup):(?<data>\w+)");
                var subscriptionGroupMatchQuery = subscriptionGroupMatch.Groups["query"].Value;
                var subscriptionGroupMatchData = subscriptionGroupMatch.Groups["data"].Value;

                if (subscriptionGroupMatch.Success && subscriptionGroupMatchQuery.Equals("subscriptionGroup"))
                    subscriptionGroup = subscriptionGroupMatchData;

                return subscriptionGroup;
            }

            private static string GetSubscriptionPeriodDataFromCallbackQuery(string callbackQueryData)
            {
                var subscriptionPeriod = string.Empty;
                var subscriptionPeriodMatch = Regex.Match(callbackQueryData, @"(?i)(?<query>subscriptionPeriod):(?<data>\w+)");
                var subscriptionPeriodMatchQuery = subscriptionPeriodMatch.Groups["query"].Value;
                var subscriptionPeriodMatchData = subscriptionPeriodMatch.Groups["data"].Value;

                if (subscriptionPeriodMatch.Success && subscriptionPeriodMatchQuery.Equals("subscriptionPeriod"))
                    subscriptionPeriod = subscriptionPeriodMatchData;

                return subscriptionPeriod;
            }

            internal static async Task GetZoukUserSubscriptionInformation(Message message, TelegramBotClient client, IUnitOfWork services)
            {
                var chatId = message.From.Id;
                var zoukUserSubscription = await GetZoukUserSubscription(message.From.Username, services);

                if (zoukUserSubscription == null)
                {
                    await GetNewZoukUserSubscriptionInformation(client, chatId);
                    return;
                }

                await GetExistingZoukUserSubscriptionInformation(client, zoukUserSubscription, chatId);
            }

            private static async Task<ZoukUserSubscription> GetZoukUserSubscription(string userName, IUnitOfWork services)
            {
                return await services.ZoukUsersSubscriptions.FindOneAsync(x =>
                    x.ZoukUser.NickName == userName
                    && x.RemainingClassesCount > 0
                    && x.Subscription.IsActive);
            }

            private static async Task GetExistingZoukUserSubscriptionInformation(TelegramBotClient client, ZoukUserSubscription zoukUserSubscription, int chatId)
            {
                await client.SendTextMessageAsync(chatId, $"Your subscription:\n{zoukUserSubscription.Subscription}", ParseMode.Markdown);
                await client.SendTextMessageAsync(chatId, "Do you want to /checkin class?", ParseMode.Markdown);
            }

            private static async Task GetNewZoukUserSubscriptionInformation(TelegramBotClient client, int chatId)
            {
                const string responseMessage = "*Which subscription do you want choose?\n*";
                var replyKeyboardMarkup = RenderSubscriptionGroups();

                await client.SendTextMessageAsync(chatId, responseMessage, ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
            }

            private static InlineKeyboardMarkup RenderSubscriptionGroups()
            {
                return InlineKeyboardBuilder.Create()
                    .AddButton("Novice group", "subscriptionGroup:Novice")
                    .AddButton("Medium group", "subscriptionGroup:Medium")
                    .AddButton("Lady Style group", "subscriptionGroup:Lady")
                    .NewLine()
                    .AddButton("Novice and Medium group", "subscriptionGroup:NoviceMedium")
                    .NewLine()
                    .AddButton("Novice and Lady Style group", "subscriptionGroup:NoviceLady")
                    .NewLine()
                    .AddButton("Medium and Lady Style group", "subscriptionGroup:MediumLady")
                    .Build();
            }

            private static InlineKeyboardMarkup RenderSubscriptionPeriods(string subscriptionGroupData)
            {
                return InlineKeyboardBuilder.Create()
                    .AddButton("Day", $"{subscriptionGroupData}&subscriptionPeriod:Day")
                    .NewLine()
                    .AddButton("Week", $"{subscriptionGroupData}&subscriptionPeriod:Week")
                    .NewLine()
                    .AddButton("Two Weeks", $"{subscriptionGroupData}&subscriptionPeriod:TwoWeeks")
                    .NewLine()
                    .AddButton("Month", $"{subscriptionGroupData}&subscriptionPeriod:Month")
                    .NewLine()
                    .AddButton("Three Months", $"{subscriptionGroupData}&subscriptionPeriod:ThreeMonths")
                    .Build();
            }

            private static InlineKeyboardMarkup RenderSubscription(ObjectId id)
            {
                return InlineKeyboardBuilder.Create()
                    .AddUrlButton("Buy", $"subscriptionPayment:{id}", "https://send.monobank.ua/3EVPSXNq4F")
                    .Build();
            }

            private enum SubscriptionCallbackQueryType
            {
                Period = 0,
                Group
            }
        }
    }
}

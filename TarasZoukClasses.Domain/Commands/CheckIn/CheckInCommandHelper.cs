namespace TarasZoukClasses.Domain.Commands.CheckIn
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Data.Models;
    using MongoDB.Bson;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;
    using Utils;

    public static class CheckInCommandHelper
    {
        internal static async Task ShowZoukUserSubscriptionsInformation(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            var chatId = message.Chat.Id;
            var zoukUserSubscriptions = await GetZoukUserSubscriptions(services, message.From.Username);
            var replyMessage = RenderSubscriptionAvailabilityText(zoukUserSubscriptions);

            await client.SendTextMessageAsync(chatId, replyMessage, ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove());

            foreach (var zoukUsersubscription in zoukUserSubscriptions)
            {
                await SendCheckInInformation(client, zoukUsersubscription, chatId);
            }

            if (zoukUserSubscriptions.Any())
                await client.SendTextMessageAsync(chatId, "*Press check-in button on the subscription where you want the class to be taken from*", ParseMode.Markdown);
        }

        private static async Task<List<ZoukUserSubscription>> GetZoukUserSubscriptions(IUnitOfWork services, string username)
        {
            return (await services.ZoukUsersSubscriptions
                .FilterBy(x => x.ZoukUser.NickName.Equals(username) && x.RemainingClassesCount > 0))
                .ToList();
        }

        private static async Task SendCheckInInformation(TelegramBotClient client, ZoukUserSubscription zoukUserSubscription, long chatId)
        {
            var replyText = RenderSubscriptionInformationText(zoukUserSubscription);
            var replyMarkup = RenderSubscriptionMarkup(zoukUserSubscription);
            await client.SendTextMessageAsync(chatId, replyText, ParseMode.Markdown, replyMarkup: replyMarkup);
        }

        private static string RenderSubscriptionAvailabilityText(List<ZoukUserSubscription> zoukUserSubscriptions)
        {
            return zoukUserSubscriptions.Any()
                ? zoukUserSubscriptions.Count == 1 ? "*Your subscription:*" : "*Your subscriptions:*"
                : "You have no subscriptions. Press /mysubscriptions to buy one.";
        }

        private static string RenderSubscriptionInformationText(ZoukUserSubscription zoukUserSubscription)
        {
            return "*Subscription:\n*" +
                   $"Name: {zoukUserSubscription.Subscription.Name}\n" +
                   $"SubscriptionType: {zoukUserSubscription.Subscription.Type}\n" +
                   $"Remaining Classes: {zoukUserSubscription.RemainingClassesCount}\n";
        }

        private static InlineKeyboardMarkup RenderSubscriptionMarkup(ZoukUserSubscription zoukUserSubscription)
        {
            return InlineKeyboardBuilder.Create()
                .AddButton("Check-in", $"checkinZoukUserSubscriptionId:{zoukUserSubscription.Id}")
                .Build();
        }

        internal static ObjectId GetZoukUserSubscriptionIdFromCallbackQuery(string callbackQueryData, string callbackQueryPattern)
        {
            var zoukUserSubscriptionIdGroup = string.Empty;
            var zoukUserSubscriptionIdGroupMatch = Regex.Match(callbackQueryData, callbackQueryPattern);
            var zoukUserSubscriptionIdGroupMatchQuery = zoukUserSubscriptionIdGroupMatch.Groups["query"].Value;
            var zoukUserSubscriptionIdGroupMatchData = zoukUserSubscriptionIdGroupMatch.Groups["data"].Value;

            if (zoukUserSubscriptionIdGroupMatch.Success && zoukUserSubscriptionIdGroupMatchQuery.Equals("checkinZoukUserSubscriptionId"))
                zoukUserSubscriptionIdGroup = zoukUserSubscriptionIdGroupMatchData;

            return new ObjectId(zoukUserSubscriptionIdGroup);
        }
    }
}

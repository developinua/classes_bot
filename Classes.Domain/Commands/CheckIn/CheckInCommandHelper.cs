using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Domain.Repositories;
using Classes.Domain.Utils;
using MongoDB.Bson;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Commands.CheckIn;

public static class CheckInCommandHelper
{
	internal static async Task ShowUserSubscriptionsInformation(
		Message message, ITelegramBotClient client, IUnitOfWork services)
	{
		var chatId = message.Chat.Id;
		var userSubscriptions = await GetUserSubscriptions(services, message.From!.Username!);
		var replyMessage = RenderSubscriptionAvailabilityText(userSubscriptions);

		await client.SendTextMessageAsync(
			chatId, replyMessage, ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove());

		foreach (var userSubscription in userSubscriptions)
		{
			await SendCheckInInformation(client, userSubscription, chatId);
		}

		if (userSubscriptions.Any())
			await client.SendTextMessageAsync(chatId,
				"*Press check-in button on the subscription where you want the class to be taken from*",
				ParseMode.Markdown);
	}

	private static async Task<List<UserSubscription>> GetUserSubscriptions(IUnitOfWork services, string username) =>
		(await services.UsersSubscriptions
			.FilterBy(x => x.User.NickName.Equals(username) && x.RemainingClassesCount > 0))
		.ToList();

	private static async Task SendCheckInInformation(
		ITelegramBotClient client, UserSubscription userSubscription, long chatId)
	{
		var replyText = RenderSubscriptionInformationText(userSubscription);
		var replyMarkup = RenderSubscriptionMarkup(userSubscription);
		await client.SendTextMessageAsync(chatId, replyText, ParseMode.Markdown, replyMarkup: replyMarkup);
	}

	private static string RenderSubscriptionAvailabilityText(IReadOnlyCollection<UserSubscription> userSubscriptions) =>
		userSubscriptions.Any()
			? userSubscriptions.Count == 1 ? "*Your subscription:*" : "*Your subscriptions:*"
			: "You have no subscriptions. Press /my-subscriptions to buy one.";

	private static string RenderSubscriptionInformationText(UserSubscription userSubscription) =>
		"*Subscription:\n*" +
		$"Name: {userSubscription.Subscription.Name}\n" +
		$"SubscriptionType: {userSubscription.Subscription.Type}\n" +
		$"Remaining Classes: {userSubscription.RemainingClassesCount}\n";

	private static InlineKeyboardMarkup RenderSubscriptionMarkup(UserSubscription userSubscription) =>
		InlineKeyboardBuilder.Create()
			.AddButton("Check-in", $"check-in-subscription-id:{userSubscription.Id}")
			.Build();

	internal static ObjectId GetUserSubscriptionIdFromCallbackQuery(
		string callbackQueryData, string callbackQueryPattern)
	{
		var userSubscriptionIdGroup = string.Empty;
		var userSubscriptionIdGroupMatch = Regex.Match(callbackQueryData, callbackQueryPattern);
		var userSubscriptionIdGroupMatchQuery = userSubscriptionIdGroupMatch.Groups["query"].Value;
		var userSubscriptionIdGroupMatchData = userSubscriptionIdGroupMatch.Groups["data"].Value;

		if (userSubscriptionIdGroupMatch.Success
			&& userSubscriptionIdGroupMatchQuery.Equals("checkinUserSubscriptionId"))
			userSubscriptionIdGroup = userSubscriptionIdGroupMatchData;

		return new ObjectId(userSubscriptionIdGroup);
	}
}
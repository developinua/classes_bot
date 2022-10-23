using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Classes.Data.Models;
using Classes.Data.Models.Enums;
using Classes.Domain.Repositories;
using Classes.Domain.Utils;
using MongoDB.Bson;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Commands.MySubscriptions;

public static class MySubscriptionsCommandHelper
{
	internal static async Task ParseSubscription(long chatId, CallbackQuery callbackQuery,
		ITelegramBotClient client, IUnitOfWork services)
	{
		var subscriptionCallbackQueryType = GetSubscriptionCallbackQueryType(callbackQuery.Data!);

		var temp = subscriptionCallbackQueryType switch
		{
			MySubscriptionsCallbackQueryType.Group =>
				SendSubscriptionGroupTextMessage(chatId, client, callbackQuery.Data!),
			MySubscriptionsCallbackQueryType.Period =>
				await SendSubscriptionPeriodTextMessage(chatId, client, services, callbackQuery.Data!),
			_ => null
		};

		if (temp is null)
			throw new NotImplementedException("No such subscription callback query type was founded.");

		await temp.Invoke();
	}

	private static MySubscriptionsCallbackQueryType GetSubscriptionCallbackQueryType(string callbackQueryData) =>
		callbackQueryData switch
		{
			_ when callbackQueryData.Contains("subsGroup") && !callbackQueryData.Contains("subsPeriod") =>
				MySubscriptionsCallbackQueryType.Group,
			_ when callbackQueryData.Contains("subsGroup") && callbackQueryData.Contains("subsPeriod") =>
				MySubscriptionsCallbackQueryType.Period,
			_ => throw new InvalidDataContractException(
				"Can't parse subscription group from user callback query data.")
		};

	private static string RenderUserSubscriptionInformationText(UserSubscription userSubscription) =>
		$"Subscription: {userSubscription.Subscription.Name}\n" +
		$"Description: {userSubscription.Subscription.Description}\n" +
		$"SubscriptionType: {userSubscription.Subscription.Type}\n" +
		$"Remaining Classes: {userSubscription.RemainingClassesCount}\n";

	private static Func<Task<Message>> SendSubscriptionGroupTextMessage(long chatId, ITelegramBotClient client,
		string callbackQueryData)
	{
		var replyKeyboardMarkup = RenderSubscriptionPeriods(callbackQueryData);
		return () => client.SendTextMessageAsync(chatId, "*Which subscription period do you prefer?\n*",
			ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
	}

	private static async Task<Func<Task<Message>>> SendSubscriptionPeriodTextMessage(
		long chatId, ITelegramBotClient client, IUnitOfWork services, string callbackQueryData)
	{
		Enum.TryParse(GetSubscriptionGroupData(callbackQueryData), true, out SubscriptionType subsGroup);
		Enum.TryParse(GetSubscriptionPeriodData(callbackQueryData), true, out SubscriptionPeriod subsPeriod);

		var subscription = await services.Subscriptions.FindOneAsync(x =>
			x.IsActive && x.Type.Equals(subsGroup) && x.Period.Equals(subsPeriod));

		if (subscription is null)
			return async () => await client.SendTextMessageAsync(chatId,
				"No available subscription was founded.\nPlease contact @nazikBro");

		await client.SendTextMessageAsync(chatId,
			$"*Price: {subscription.GetSummaryPrice()}\n*P.S. Please send your username and subscription in comment",
			ParseMode.Markdown, replyMarkup: RenderBuySubscription(subscription.Id));

		return async () => await client.SendTextMessageAsync(chatId,
			"*After your subscription will be approved by teacher\nYou will be able to /checkin on classes.*",
			ParseMode.Markdown);
	}

	private static string GetSubscriptionGroupData(string callbackQueryData)
	{
		var subscriptionGroup = string.Empty;
		var subscriptionGroupMatch = Regex.Match(callbackQueryData, @"(?i)(?<query>subsGroup):(?<data>\w+)");
		var subscriptionGroupMatchQuery = subscriptionGroupMatch.Groups["query"].Value;
		var subscriptionGroupMatchData = subscriptionGroupMatch.Groups["data"].Value;

		if (subscriptionGroupMatch.Success && subscriptionGroupMatchQuery.Equals("subsGroup"))
			subscriptionGroup = subscriptionGroupMatchData;

		return subscriptionGroup;
	}

	private static string GetSubscriptionPeriodData(string callbackQueryData)
	{
		var subscriptionPeriod = string.Empty;
		var subscriptionPeriodMatch = Regex.Match(callbackQueryData, @"(?i)(?<query>subsPeriod):(?<data>\w+)");
		var subscriptionPeriodMatchQuery = subscriptionPeriodMatch.Groups["query"].Value;
		var subscriptionPeriodMatchData = subscriptionPeriodMatch.Groups["data"].Value;

		if (subscriptionPeriodMatch.Success && subscriptionPeriodMatchQuery.Equals("subsPeriod"))
			subscriptionPeriod = subscriptionPeriodMatchData;

		return subscriptionPeriod;
	}

	internal static async Task GetUserSubscriptionInformation(
		Message message, ITelegramBotClient client, IUnitOfWork services)
	{
		var chatId = message.From!.Id;
		var userSubscriptions = (await GetUserSubscriptions(message.From.Username!, services)).ToList();

		if (!userSubscriptions.Any())
		{
			await GetNewUserSubscriptionInformation(client, chatId);
			return;
		}

		await GetExistingUserSubscriptionInformation(client, userSubscriptions, chatId);
	}

	private static async Task<IEnumerable<UserSubscription>> GetUserSubscriptions(
		string userName, IUnitOfWork services) =>
		await services.UsersSubscriptions.FilterBy(x =>
			x.User.NickName == userName
			&& x.RemainingClassesCount > 0
			&& x.Subscription.IsActive);

	private static async Task GetExistingUserSubscriptionInformation(
		ITelegramBotClient client, IReadOnlyCollection<UserSubscription> userSubscriptions, long chatId)
	{
		var pluralEnding = userSubscriptions.Count > 1 ? "s" : "";
		await client.SendTextMessageAsync(chatId, $"*Your subscription{pluralEnding}:*", ParseMode.Markdown);

		foreach (var replyMessage in userSubscriptions.Select(RenderUserSubscriptionInformationText))
			await client.SendTextMessageAsync(chatId, replyMessage, ParseMode.Markdown);

		// TODO: Change to add functionality for adding few subscriptions
		await client.SendTextMessageAsync(chatId, "*Do you want to /checkin class?*", ParseMode.Markdown);
	}

	private static async Task GetNewUserSubscriptionInformation(ITelegramBotClient client, long chatId)
	{
		const string responseMessage = "*Which subscription do you want choose?\n*";
		var replyKeyboardMarkup = RenderSubscriptionGroups();

		await client.SendTextMessageAsync(chatId, responseMessage, ParseMode.Markdown,
			replyMarkup: replyKeyboardMarkup);
	}

	private static InlineKeyboardMarkup RenderSubscriptionGroups() =>
		InlineKeyboardBuilder.Create()
			.AddButton("Novice subscription", "subsGroup:novice").NewLine()
			.AddButton("Medium subscription", "subsGroup:medium").NewLine()
			.AddButton("Lady style subscription", "subsGroup:lady").NewLine()
			.AddButton("Novice and medium subscription", "subsGroup:novice-medium").NewLine()
			.AddButton("Novice and lady style subscription", "subsGroup:novice-lady").NewLine()
			.AddButton("Medium and lady style subscription", "subsGroup:medium-lady").NewLine()
			.AddButton("Premium", "subsGroup:premium")
			.Build();

	private static InlineKeyboardMarkup RenderSubscriptionPeriods(string subscriptionGroupData)
	{
		var subscriptionPeriods = InlineKeyboardBuilder.Create();

		if (!subscriptionGroupData.Contains("Premium"))
			subscriptionPeriods.AddButton("Day", $"{subscriptionGroupData}?subsPeriod:day").NewLine();

		subscriptionPeriods.AddButton("Week", $"{subscriptionGroupData}?subsPeriod:week").NewLine()
			.AddButton("Two weeks", $"{subscriptionGroupData}?subsPeriod:two-weeks").NewLine()
			.AddButton("Month", $"{subscriptionGroupData}?subsPeriod:month").NewLine()
			.AddButton("Three months", $"{subscriptionGroupData}?subsPeriod:three-months");

		return subscriptionPeriods.Build();
	}

	private static InlineKeyboardMarkup RenderBuySubscription(ObjectId id)
	{
		// TODO: Add replacement link
		return InlineKeyboardBuilder.Create()
			.AddUrlButton("Buy", $"paymentlink:{id}", "https://send.monobank.ua/4aXrhJ1FTH")
			.Build();
	}
}
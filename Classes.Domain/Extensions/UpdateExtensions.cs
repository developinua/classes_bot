using Classes.Domain.Handlers.UpdateHandler;
using Classes.Domain.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Extensions;

public static class UpdateExtensions
{
	public static IUpdateHandler? GetHandler(
		this Update update, 
		ITelegramBotClient botClient,
		IUnitOfWork db) =>
		update switch
		{
			{Type: UpdateType.Message, Message.Type: MessageType.Location} =>
				new LocationUpdateHandler(botClient, db),
			{Type: UpdateType.Message} => new MessageUpdateHandler(botClient, db),
			{Type: UpdateType.CallbackQuery} => new CallbackQueryUpdateHandler(botClient, db),
			{Type: UpdateType.Unknown} => null,
			{Type: UpdateType.InlineQuery} => null,
			{Type: UpdateType.ChosenInlineResult} => null,
			{Type: UpdateType.EditedMessage} => null,
			{Type: UpdateType.ChannelPost} => null,
			{Type: UpdateType.EditedChannelPost} => null,
			{Type: UpdateType.ShippingQuery} => null,
			{Type: UpdateType.PreCheckoutQuery} => null,
			{Type: UpdateType.Poll} => null,
			{Type: UpdateType.PollAnswer} => null,
			_ => null
		};
}
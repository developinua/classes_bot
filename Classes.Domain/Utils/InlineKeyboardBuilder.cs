using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Utils;

public class InlineKeyboardBuilder
{
	private List<InlineKeyboardButton> Rows { get; set; } = new();
	private readonly List<List<InlineKeyboardButton>> _keyboard = new();

	public static InlineKeyboardBuilder Create() => new();

	public InlineKeyboardBuilder AddButton(string text, string callbackData)
	{
		Rows.Add(new InlineKeyboardButton(text) {CallbackData = callbackData});
		return this;
	}

	public InlineKeyboardBuilder AddUrlButton(string text, string callbackData, string url)
	{
		Rows.Add(new InlineKeyboardButton(text)
		{
			CallbackData = callbackData,
			Pay = true,
			Url = url
		});

		return this;
	}

	public InlineKeyboardBuilder NewLine()
	{
		_keyboard.Add(Rows);
		Rows = new List<InlineKeyboardButton>();

		return this;
	}

	public InlineKeyboardMarkup Build()
	{
		_keyboard.Add(Rows);
		return new InlineKeyboardMarkup(_keyboard);
	}
}
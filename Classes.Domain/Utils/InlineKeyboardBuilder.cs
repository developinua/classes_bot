using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Classes.Domain.Utils;

public class InlineKeyboardBuilder
{
	private List<List<InlineKeyboardButton>> Keyboard { get; }
	private List<InlineKeyboardButton> Rows { get; set; }

	private InlineKeyboardBuilder()
	{
		Keyboard = new List<List<InlineKeyboardButton>>();
		Rows = new List<InlineKeyboardButton>();
	}

	public static InlineKeyboardBuilder Create()
	{
		return new InlineKeyboardBuilder();
	}

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
		Keyboard.Add(Rows);
		Rows = new List<InlineKeyboardButton>();

		return this;
	}

	public InlineKeyboardMarkup Build()
	{
		Keyboard.Add(Rows);
		return new InlineKeyboardMarkup(Keyboard);
	}
}
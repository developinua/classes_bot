using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Validators;

public static class CallbackQueryValidator
{
    public static bool Validate(this CallbackQuery callbackQuery) =>
		callbackQuery.From.IsBot
		|| callbackQuery.Message.Chat.Type != ChatType.Private
		|| callbackQuery.Message.ForwardFromChat != null;
}
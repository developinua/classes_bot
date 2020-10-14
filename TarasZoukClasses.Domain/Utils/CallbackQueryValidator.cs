namespace TarasZoukClasses.Domain.Utils
{
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public static class CallbackQueryValidator
    {
        public static bool Validate(this CallbackQuery callbackQuery)
        {
            return callbackQuery.From.IsBot
                   || callbackQuery.Message.Chat.Type != ChatType.Private
                   || callbackQuery.Message.ForwardFromChat != null;
        }
    }
}

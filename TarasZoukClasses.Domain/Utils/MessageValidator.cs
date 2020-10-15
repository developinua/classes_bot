namespace TarasZoukClasses.Domain.Utils
{
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public static class MessageValidator
    {
        public static bool ValidateMessageLocationData(this Message message)
        {
            return message?.ReplyToMessage?.From?.Username == "TarasZoukClassesBot"
                   && message.Type.Equals(MessageType.Location)
                   && message.Location != null;
        }
    }
}

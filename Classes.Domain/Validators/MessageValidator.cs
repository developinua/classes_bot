using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Validators;

public static class MessageValidator
{
    public static bool ValidateMessageLocationData(this Message message) =>
        //message?.ReplyToMessage?.From?.Username == "ZoukClassesBot" &&
        message.Type.Equals(MessageType.Location)
        && message.Location != null;
}
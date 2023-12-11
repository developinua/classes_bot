using FluentValidation;
using Telegram.Bot.Types;

namespace Classes.Domain.Validators;

public class MessageValidator : AbstractValidator<Message>
{
    public MessageValidator()
    {
        // RuleFor(x => x)
        //     .Must(x =>
        //         // x?.ReplyToMessage?.From?.Username == "ClassesBot" &&
        //         x.Type.Equals(MessageType.Location)
        //         && x.Location != null)
        //     .WithMessage("Must be valid message");
    }
}
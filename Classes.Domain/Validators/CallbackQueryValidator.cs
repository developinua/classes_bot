using FluentValidation;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Validators;

public class CallbackQueryValidator : AbstractValidator<CallbackQuery>
{
	public CallbackQueryValidator()
	{
		RuleFor(x => x)
			.Must(x =>
				x.From.IsBot
				|| x.Message!.Chat.Type != ChatType.Private
				|| x.Message.ForwardFromChat != null)
			.WithMessage("Must be valid callback query");
	}
}
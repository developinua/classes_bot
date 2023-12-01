using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Commands;
using Classes.Domain.Handlers.Checkin;
using FluentValidation;
using ResultNet;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers.UpdateMessage;

public class LocationHandler : IUpdateHandler
{
    private readonly IEnumerable<IBotCommand> _commands;
    private readonly IValidator<Message> _messageValidator;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly PostgresDbContext _dbContext;

    public LocationHandler(
        IEnumerable<IBotCommand> commands,
        IValidator<Message> validator,
        ITelegramBotClient telegramBotClient,
        PostgresDbContext dbContext)
    {
        _commands = commands;
        _messageValidator = validator;
        _telegramBotClient = telegramBotClient;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(Update update)
    {
        var validationResult = await _messageValidator.ValidateAsync(update.Message ?? new());

        if (!validationResult.IsValid)
            return Result.Failure().WithMessage($"Message update is null. {DateTime.UtcNow}.");

        await _commands
            .Single(x => x.GetType() == typeof(CheckinHandler))
            .Execute(update.Message!, _telegramBotClient, _dbContext);

        return Result.Success();
    }
}
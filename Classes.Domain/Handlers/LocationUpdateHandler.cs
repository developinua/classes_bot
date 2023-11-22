using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Commands;
using FluentValidation;
using ResultNet;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers;

public class LocationUpdateHandler : IUpdateHandler
{
    private readonly IEnumerable<IBotCommand> _commands;
    private readonly IValidator<Message> _validator;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly PostgresDbContext _dbContext;

    public LocationUpdateHandler(
        IEnumerable<IBotCommand> commands,
        IValidator<Message> validator,
        ITelegramBotClient telegramBotClient,
        PostgresDbContext dbContext)
    {
        _commands = commands;
        _validator = validator;
        _telegramBotClient = telegramBotClient;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(Update update)
    {
        var isLocationDataValid = (await _validator.ValidateAsync(update.Message ?? new())).IsValid;

        if (!isLocationDataValid)
            return Result.Failure().WithMessage($"Message update is null. {DateTime.UtcNow}.");

        await _commands
            .Single(x => x.Name == "/check-in")
            .Execute(update.Message!, _telegramBotClient, _dbContext);

        return Result.Success();
    }
}
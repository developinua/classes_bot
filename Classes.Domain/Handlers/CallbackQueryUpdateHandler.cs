using System;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Repositories;
using ResultNet;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers;

public class CallbackQueryUpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICommandRepository _commandRepository;
    private readonly PostgresDbContext _dbContext;

    public CallbackQueryUpdateHandler(
        ITelegramBotClient telegramBotClient,
        PostgresDbContext dbContext,
        ICommandRepository commandRepository)
    {
        _commandRepository = commandRepository;
        _telegramBotClient = telegramBotClient;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(Update update)
    {
        var callbackQuery = update.CallbackQuery;

        if (callbackQuery == null)
        {
            return Result.Failure()
                .WithMessage($"Message update is null. {DateTime.UtcNow}.");
        }

        var commands = await _commandRepository.GetActiveCommandsAsync();
        var userCommand = commands.SingleOrDefault(command => command.Contains(callbackQuery.Data!));

        if (userCommand == null)
        {
            return Result.Failure()
                .WithMessage($"This bot can't process callback query: {callbackQuery.Data}.");
        }

        await userCommand.Execute(callbackQuery, _telegramBotClient, _dbContext);

        return Result.Success();
    }
}
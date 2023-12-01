using System;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Repositories;
using ResultNet;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers.UpdateMessage;

public class CallbackQueryHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICommandRepository _commandRepository;
    private readonly PostgresDbContext _dbContext;

    public CallbackQueryHandler(
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
        if (update.CallbackQuery == null)
        {
            return Result.Failure()
                .WithMessage($"Message update is null. {DateTime.UtcNow}.");
        }

        var commands = await _commandRepository.GetActiveCommandsAsync();
        var userCommand = commands.SingleOrDefault(command => command.Contains(update.CallbackQuery.Data!));

        if (userCommand == null)
        {
            return Result.Failure()
                .WithMessage($"This bot can't process callback query: {update.CallbackQuery.Data}.");
        }

        await userCommand.Execute(update.CallbackQuery, _telegramBotClient, _dbContext);

        return Result.Success();
    }
}
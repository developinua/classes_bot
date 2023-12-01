using System;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Data.Repositories;
using Newtonsoft.Json;
using ResultNet;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers.UpdateMessage;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICommandRepository _commandRepository;
    private readonly PostgresDbContext _dbContext;

    public UpdateHandler(
        ITelegramBotClient telegramBotClient,
        PostgresDbContext dbContext,
        ICommandRepository commandRepository)
    {
        _telegramBotClient = telegramBotClient;
        _dbContext = dbContext;
        _commandRepository = commandRepository;
    }

    public async Task<Result> Handle(Update update)
    {
        if (update.Message == null)
        {
            return Result.Failure()
                .WithMessage($"Message update is null. {DateTime.UtcNow}.");
        }

        var commands = await _commandRepository.GetActiveCommandsAsync();
        var userCommand = commands.SingleOrDefault(command => command.Contains(update.Message));

        if (userCommand == null)
        {
            return Result.Failure()
                .WithMessage($"Can't process message {JsonConvert.SerializeObject(update.Message)}");
        }

        await userCommand.Execute(update.Message, _telegramBotClient, _dbContext);

        return Result.Success();
    }
}
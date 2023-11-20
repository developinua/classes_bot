using System;
using System.Linq;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Repositories;
using Newtonsoft.Json;
using ResultNet;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers;

public class MessageUpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICommandRepository _commandRepository;
    private readonly PostgresDbContext _dbContext;

    public MessageUpdateHandler(
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
        var message = update.Message;

        if (message == null)
        {
            return Result.Failure()
                .WithMessage($"Message update is null. {DateTime.UtcNow}.");
        }

        var commands = await _commandRepository.GetActiveCommandsAsync();
        var userCommand = commands.SingleOrDefault(command => command.Contains(message));

        if (userCommand == null)
        {
            return Result.Failure()
                .WithMessage($"Can't process message {JsonConvert.SerializeObject(message)}");
        }

        await userCommand.Execute(message, _telegramBotClient, _dbContext);

        return Result.Success();
    }
}
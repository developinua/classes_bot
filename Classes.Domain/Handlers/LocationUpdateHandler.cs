using System;
using System.Threading.Tasks;
using Classes.Data.Context;
using Classes.Domain.Commands.CheckIn;
using Classes.Domain.Validators;
using ResultNet;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers;

public class LocationUpdateHandler : IUpdateHandler
{
    private ITelegramBotClient TelegramBotClient { get; }
    private PostgresDbContext DbContext { get; }

    public LocationUpdateHandler(ITelegramBotClient telegramBotClient, PostgresDbContext dbContext)
    {
        TelegramBotClient = telegramBotClient;
        DbContext = dbContext;
    }

    public async Task<Result> Handle(Update update)
    {
        var isLocationDataValid = update.Message!.ValidateMessageLocationData();

        if (!isLocationDataValid)
        {
            return Result.Failure()
                .WithMessage($"Message update is null. {DateTime.UtcNow}.");
        }

        await new CheckInCommand().Execute(update.Message!, TelegramBotClient, DbContext);

        return Result.Success();
    }
}
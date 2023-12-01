using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultNet;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Services;

public interface IUpdateService
{
    Result<IRequest<Result>> GetRequestFromUpdate(Update update);
}

public class UpdateService : IUpdateService
{
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(IMapper mapper, ILogger<UpdateService> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    public Result<IRequest<Result>> GetRequestFromUpdate(Update update)
    {
        var request = update switch
        {
            { Type: UpdateType.Message } => _mapper.Map<IRequest<Result>>(update.Message!),
            { Type: UpdateType.CallbackQuery } => _mapper.Map<IRequest<Result>>(update.CallbackQuery),
            _ => null
        };

        if (request is null)
        {
            _logger.LogError(
                "Update handler not found\n" +
                "Update type: {updateType}\n" +
                "Update message type: {messageType}",
                update.Type,
                update.Message?.Type.ToString() ?? "No message type was specified");
            return Result.Failure<IRequest<Result>>()
                .WithMessage("Update handler not found");
        }

        return Result.Success(request);
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultNet;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Application.Services;

public interface IUpdateService
{
    long GetChatId(Update update);
    string GetUsername(CallbackQuery callbackQuery);
    Result<IRequest<Result>> GetRequestFromUpdate(Update update);
    Task HandleSuccessResponse(long chatId);
    Task HandleFailureResponse(long chatId, CancellationToken cancellationToken, string? responseMessage = null);
}

public class UpdateService : IUpdateService
{
    private readonly IMapper _mapper;
    private readonly IBotService _botService;
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(
        IMapper mapper,
        IBotService botService,
        ILogger<UpdateService> logger)
    {
        _mapper = mapper;
        _botService = botService;
        _logger = logger;
    }

    public long GetChatId(Update update) =>
        update.Message?.From?.Id ?? update.CallbackQuery!.From.Id;

    public string GetUsername(CallbackQuery callbackQuery) =>
        callbackQuery.From.Username ?? callbackQuery.From.Id.ToString();

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
    
    public Task HandleSuccessResponse(long chatId)
    {
        _logger.LogInformation(
            "Successful response from chat {chatId}. Date: {dateTime}",
            chatId.ToString(),
            DateTime.UtcNow);
        return Task.CompletedTask;
    }
    
    public async Task HandleFailureResponse(
        long chatId,
        CancellationToken cancellationToken,
        string? responseMessage = null)
    {
        _logger.LogError(
            "Chat id: {chatId}\nMessage:\n{errorMessage}",
            chatId.ToString(),
            responseMessage ?? "No message was specified");
            
        await _botService.SendTextMessageAsync(
            chatId,
            "Can't process message",
            cancellationToken: cancellationToken);
    }
}
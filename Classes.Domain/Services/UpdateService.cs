using System.Collections.Generic;
using System.Linq;
using Classes.Domain.Handlers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Classes.Domain.Services;

public interface IUpdateService
{
    IUpdateHandler? GetHandler(Update update);
}

public class UpdateService : IUpdateService
{
    private readonly IEnumerable<IUpdateHandler> _updateHandlers;

    public UpdateService(IEnumerable<IUpdateHandler> updateHandlers) => _updateHandlers = updateHandlers;

    public IUpdateHandler? GetHandler(Update update)
    {
        var handler = update switch
        {
            { Type: UpdateType.Message, Message.Type: MessageType.Location } =>
                _updateHandlers.Single(x => x.GetType() == typeof(LocationUpdateHandler)),
            { Type: UpdateType.Message } =>
                _updateHandlers.Single(x => x.GetType() == typeof(MessageUpdateHandler)),
            { Type: UpdateType.CallbackQuery } =>
                _updateHandlers.Single(x => x.GetType() == typeof(CallbackQueryUpdateHandler)),
            { Type: UpdateType.InlineQuery } => null,
            { Type: UpdateType.ChosenInlineResult } => null,
            { Type: UpdateType.EditedMessage } => null,
            { Type: UpdateType.ChannelPost } => null,
            { Type: UpdateType.EditedChannelPost } => null,
            { Type: UpdateType.ShippingQuery } => null,
            { Type: UpdateType.PreCheckoutQuery } => null,
            { Type: UpdateType.Poll } => null,
            { Type: UpdateType.PollAnswer } => null,
            { Type: UpdateType.Unknown } => null,
            _ => null
        };
        return handler;
    }
}
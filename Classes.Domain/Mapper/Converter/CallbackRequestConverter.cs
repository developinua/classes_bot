using AutoMapper;
using Classes.Domain.BotRequest;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Mapper.Converter;

public class CallbackRequestValueConverter : ITypeConverter<CallbackQuery, IRequest<Result>?>
{
    private readonly IEnumerable<BotCallbackRequest> _callbackRequests;

    public CallbackRequestValueConverter(IEnumerable<BotCallbackRequest> callbackRequests) =>
        _callbackRequests = callbackRequests;

    public IRequest<Result>? Convert(CallbackQuery source, IRequest<Result>? destination, ResolutionContext context)
    {
        var callbackRequest = _callbackRequests.SingleOrDefault(x => x.Contains(source.Data!));

        if (callbackRequest is null) return default;
        
        return context.Mapper.Map(source, callbackRequest, typeof(CallbackQuery), callbackRequest.GetType())
            as IRequest<Result>;
    }
}
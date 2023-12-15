using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Classes.Domain.Requests.Bot;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Mapper.Converter;

public class CallbackRequestValueConverter(IEnumerable<BotCallbackRequest> callbackRequests)
    : ITypeConverter<CallbackQuery, IRequest<Result>?>
{
    public IRequest<Result>? Convert(CallbackQuery source, IRequest<Result>? destination, ResolutionContext context)
    {
        var callbackRequest = callbackRequests.SingleOrDefault(x => x.Contains(source.Data!));

        if (callbackRequest is null) return default;
        
        return context.Mapper.Map(
                source,
                callbackRequest,
                typeof(CallbackQuery),
                callbackRequest.GetType())
            as IRequest<Result>;
    }
}
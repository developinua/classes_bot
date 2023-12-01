using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Classes.Domain.Models.BotRequest;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Mapper.Resolver;

public class CallbackRequestValueResolver : IValueResolver<CallbackQuery, IRequest<Result>?, IRequest<Result>?>
{
    private readonly IEnumerable<BotCallbackRequest> _callbackRequests;

    public CallbackRequestValueResolver(IEnumerable<BotCallbackRequest> callbackRequests) =>
        _callbackRequests = callbackRequests;

    public IRequest<Result>? Resolve(
        CallbackQuery src,
        IRequest<Result>? destination,
        IRequest<Result>? destMember,
        ResolutionContext context)
    {
        var callbackRequest = _callbackRequests.SingleOrDefault(x => x.Contains(src.Data!));

        if (callbackRequest is null) return default;
        
        return context.Mapper.Map(src, callbackRequest, typeof(CallbackQuery), callbackRequest.GetType())
            as IRequest<Result>;
    }
}
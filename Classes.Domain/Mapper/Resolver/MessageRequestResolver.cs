using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Classes.Domain.Models.BotRequest;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Mapper.Resolver;

public class MessageRequestValueResolver : IValueResolver<Message, IRequest<Result>?, IRequest<Result>?>
{
    private readonly IEnumerable<BotMessageRequest> _messageRequests;

    public MessageRequestValueResolver(IEnumerable<BotMessageRequest> messageRequests) =>
        _messageRequests = messageRequests;

    public IRequest<Result>? Resolve(
        Message src,
        IRequest<Result>? destination,
        IRequest<Result>? destMember,
        ResolutionContext context)
    {
        var messageRequest = _messageRequests.SingleOrDefault(x => x.Contains(src));

        if (messageRequest is null) return default;
        
        return context.Mapper.Map(src, messageRequest, typeof(Message), messageRequest.GetType())
            as IRequest<Result>;
    }
}
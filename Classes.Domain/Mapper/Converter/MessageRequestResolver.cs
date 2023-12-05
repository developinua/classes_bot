using AutoMapper;
using Classes.Domain.BotRequest;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Mapper.Converter;

public class MessageRequestValueConverter : ITypeConverter<Message, IRequest<Result>?>
{
    private readonly IEnumerable<BotMessageRequest> _messageRequests;

    public MessageRequestValueConverter(IEnumerable<BotMessageRequest> messageRequests) =>
        _messageRequests = messageRequests;

    public IRequest<Result>? Convert(Message source, IRequest<Result>? destination, ResolutionContext context)
    {
        var messageRequest = _messageRequests.SingleOrDefault(x => x.Contains(source));

        if (messageRequest is null) return default;
        
        return context.Mapper.Map(source, messageRequest, typeof(Message), messageRequest.GetType())
            as IRequest<Result>;
    }
}
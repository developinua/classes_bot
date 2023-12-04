using Classes.Domain.Mapper.Converter;
using Classes.Domain.Requests;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Mapper;

public class UpdateProfile : AutoMapper.Profile
{
    public UpdateProfile()
    {
        CreateMap<Message, StartRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Chat.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(
                src => src.From == null
                    ? string.Empty
                    : src.From.Username)
            );

        CreateMap<Message, IRequest<Result>?>().ConvertUsing<MessageRequestValueConverter>();
        CreateMap<CallbackQuery, IRequest<Result>?>().ConvertUsing<CallbackRequestValueConverter>();
    }
}
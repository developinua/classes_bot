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
        CreateMap<CallbackQuery, StartCallbackRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.From.Id))
            .ForMember(dest => dest.CallbackQuery, opt => opt.MapFrom(src => src));
        
        CreateMap<Message, SubscriptionsRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Chat.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(
                src => src.From == null
                    ? string.Empty
                    : src.From.Username)
            );
        CreateMap<CallbackQuery, SubscriptionsCallbackRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.From.Id))
            .ForMember(dest => dest.CallbackQuery, opt => opt.MapFrom(src => src));
        
        CreateMap<Message, CheckinRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Chat.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(
                src => src.From == null
                    ? string.Empty
                    : src.From.Username)
            )
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src));
        CreateMap<CallbackQuery, CheckinCallbackRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.From.Id))
            .ForMember(dest => dest.CallbackQuery, opt => opt.MapFrom(src => src));
        
        CreateMap<Message, AdminRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Chat.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(
                src => src.From == null
                    ? string.Empty
                    : src.From.Username)
            );
        CreateMap<Message, SeedRequest>()
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
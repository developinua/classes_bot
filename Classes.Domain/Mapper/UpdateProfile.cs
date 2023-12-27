using System.Linq.Expressions;
using AutoMapper;
using Classes.Domain.Mapper.Converter;
using Classes.Domain.Requests;
using MediatR;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Mapper;

public class UpdateProfile : Profile
{
    public UpdateProfile()
    {
        CreateMap<StartRequest, LanguageRequest>();
        
        CreateMap<Message, StartRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetMessageChatId()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(GetMessageUsername()))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src));
        
        CreateMap<Message, LanguageRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetMessageChatId()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(GetMessageUsername()));
        CreateMap<CallbackQuery, LanguageCallbackRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetCallbackChatId()))
            .ForMember(dest => dest.CallbackQuery, opt => opt.MapFrom(src => src));
        
        CreateMap<Message, SubscriptionsRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetMessageChatId()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(GetMessageUsername()));
        CreateMap<CallbackQuery, SubscriptionsInformationCallbackRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetCallbackChatId()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(GetCallbackUsername()))
            .ForMember(dest => dest.CallbackQuery, opt => opt.MapFrom(src => src));
        CreateMap<CallbackQuery, SubscriptionsDetailedInformationCallbackRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetCallbackChatId()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(GetCallbackUsername()))
            .ForMember(dest => dest.CallbackQuery, opt => opt.MapFrom(src => src));
        
        CreateMap<Message, CheckinRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetMessageChatId()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(GetMessageUsername()))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src));
        CreateMap<CallbackQuery, CheckinCallbackRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetCallbackChatId()))
            .ForMember(dest => dest.CallbackQuery, opt => opt.MapFrom(src => src));
        
        CreateMap<Message, AdminRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetMessageChatId()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(GetMessageUsername()));
        CreateMap<Message, SeedRequest>()
            .ForMember(dest => dest.ChatId, opt => opt.MapFrom(GetMessageChatId()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(GetMessageUsername()));

        CreateMap<Message, IRequest<Result>?>().ConvertUsing<MessageRequestValueConverter>();
        CreateMap<CallbackQuery, IRequest<Result>?>().ConvertUsing<CallbackRequestValueConverter>();
        
        return;
        
        Expression<Func<Message, long?>> GetMessageChatId() => src => src.Chat.Id;
        Expression<Func<CallbackQuery, long?>> GetCallbackChatId() => src => src.From.Id;
        Expression<Func<Message, string?>> GetMessageUsername() => src =>
            src.From == null ? string.Empty : src.From.Username;
        //todo: check GetCallbackUsername src.From is null
        Expression<Func<CallbackQuery, string?>> GetCallbackUsername() => src => src.From.Username;
    }
}
namespace TarasZoukClasses.Domain.Handlers.Message
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IUpdateHandler
    {
        Task<UpdateHandlerResponse> Handle(Update update);
    }
}

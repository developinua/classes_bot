namespace TarasZoukClasses.Api.Handlers.UpdateHandler.UpdateHandlerContract
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;
    using UpdateHandlerResponse;

    public interface IUpdateHandler
    {
        Task<UpdateHandlerResponse> Handle(Update update);
    }
}

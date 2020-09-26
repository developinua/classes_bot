namespace TarasZoukClasses.Domain.Handlers.UpdateHandler.UpdateHandlerContract
{
    using System.Threading.Tasks;
    using Telegram.Bot.Types;

    public interface IUpdateHandler
    {
        Task<UpdateHandlerResponse.UpdateHandlerResponse> Handle(Update update);
    }
}

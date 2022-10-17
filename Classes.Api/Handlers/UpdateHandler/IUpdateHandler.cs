using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Classes.Api.Handlers.UpdateHandler;

public interface IUpdateHandler
{
    Task<UpdateHandlerResponse> Handle(Update update);
}
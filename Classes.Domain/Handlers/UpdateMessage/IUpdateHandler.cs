using System.Threading.Tasks;
using ResultNet;
using Telegram.Bot.Types;

namespace Classes.Domain.Handlers.UpdateMessage;

public interface IUpdateHandler
{
    Task<Result> Handle(Update update);
}
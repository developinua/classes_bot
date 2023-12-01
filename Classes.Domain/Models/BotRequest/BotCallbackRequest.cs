using System.Text.RegularExpressions;

namespace Classes.Domain.Models.BotRequest;

public abstract class BotCallbackRequest
{
    public abstract string CallbackPattern { get; }
    
    public virtual bool Contains(string callbackData) =>
        new Regex(CallbackPattern).Match(callbackData).Success;
}
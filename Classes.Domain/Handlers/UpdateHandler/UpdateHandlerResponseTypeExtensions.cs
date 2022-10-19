namespace Classes.Domain.Handlers.UpdateHandler;

public static class UpdateHandlerResponseTypeExtensions
{
    public static bool IsOk(this UpdateHandlerResponseType updateHandlerResponseType) => 
        updateHandlerResponseType == UpdateHandlerResponseType.Ok;

    public static bool IsError(this UpdateHandlerResponseType updateHandlerResponseType) => 
        updateHandlerResponseType == UpdateHandlerResponseType.Error;
}
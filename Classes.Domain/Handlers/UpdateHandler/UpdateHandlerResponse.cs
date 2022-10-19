namespace Classes.Domain.Handlers.UpdateHandler;

public class UpdateHandlerResponse
{
    public string Message { get; set; } = string.Empty;
    public UpdateHandlerResponseType ResponseType { get; set; } = UpdateHandlerResponseType.Error;
}
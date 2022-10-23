namespace Classes.Data.Models;

public class UserInformation : Document
{
    public Culture Culture { get; set; } = new();
    public long ChatId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
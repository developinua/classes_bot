namespace Classes.Data.Models;

public class User : Document
{
    public UserInformation UserInformation { get; set; } = new();
    public string NickName { get; set; } = string.Empty;
}
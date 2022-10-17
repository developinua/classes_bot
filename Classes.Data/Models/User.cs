namespace Classes.Data.Models;

public class User : Document
{
    public UserInformation UserInformation { get; set; }
    public string NickName { get; set; }
}
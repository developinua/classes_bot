namespace Classes.Data.Models;

public class Command : Document
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
}
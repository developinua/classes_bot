namespace Classes.Data.Models;

public class Command : Document
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
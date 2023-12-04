namespace Classes.Domain.Models;

public class Command : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
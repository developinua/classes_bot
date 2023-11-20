using System;

namespace Classes.Data.Models;

public class BaseModel
{
    public long Id { get; set; }
    public DateTime CreatedAt => DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
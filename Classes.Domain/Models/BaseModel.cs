using System;

namespace Classes.Domain.Models;

public class BaseModel
{
    public long Id { get; set; }
    public DateTime CreatedAt => DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
namespace BlogApplication.Domain.Entities;

/// <summary>
/// Base class for all MySQL database entities providing common audit fields.
/// </summary>
public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

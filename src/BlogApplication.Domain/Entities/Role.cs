namespace BlogApplication.Domain.Entities;

/// <summary>
/// Represents a role that can be assigned to users for authorization purposes.
/// </summary>
public class Role : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

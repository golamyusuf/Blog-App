namespace BlogApplication.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between users and roles.
/// </summary>
public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public DateTime AssignedAt { get; set; }
}

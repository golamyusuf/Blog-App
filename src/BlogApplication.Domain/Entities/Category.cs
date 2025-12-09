namespace BlogApplication.Domain.Entities;

/// <summary>
/// Represents a category for organizing blog posts.
/// Categories can be created by both administrators and regular users.
/// </summary>
public class Category : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
}

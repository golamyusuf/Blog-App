namespace BlogApplication.Domain.Entities;

public class Category : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
}

using BlogApplication.Domain.Entities;

namespace BlogApplication.Application.DTOs.Blog;

public class BlogDto
{
    public string Id { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<MediaItemDto> MediaItems { get; set; } = new();
    public int ViewCount { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class CreateBlogDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<MediaItemDto> MediaItems { get; set; } = new();
    public bool IsPublished { get; set; }
}

public class UpdateBlogDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<MediaItemDto> MediaItems { get; set; } = new();
    public bool IsPublished { get; set; }
}

public class MediaItemDto
{
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public string? Caption { get; set; }
    public int Order { get; set; }
}

public class BlogListDto
{
    public List<BlogDto> Blogs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

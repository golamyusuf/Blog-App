namespace BlogApplication.Domain.Entities;

/// <summary>
/// Represents a media item (image or video) embedded in a blog post.
/// </summary>
public class MediaItem
{
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public string? Caption { get; set; }
    public int Order { get; set; }
}

public enum MediaType
{
    Image,
    Video
}

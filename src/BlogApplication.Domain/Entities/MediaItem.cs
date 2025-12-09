namespace BlogApplication.Domain.Entities;

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

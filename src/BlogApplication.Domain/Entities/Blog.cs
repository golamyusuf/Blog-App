using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogApplication.Domain.Entities;

public class Blog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<MediaItem> MediaItems { get; set; } = new();
    public int ViewCount { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

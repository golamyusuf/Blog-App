using MongoDB.Driver;
using BlogApplication.Domain.Entities;
using BlogApplication.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace BlogApplication.Infrastructure.Data;

/// <summary>
/// MongoDB database context for storing blog posts and related media.
/// Provides access to the Blogs collection.
/// </summary>
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Blog> Blogs => _database.GetCollection<Blog>("Blogs");
}

using MongoDB.Driver;
using BlogApplication.Domain.Entities;
using BlogApplication.Domain.Interfaces;
using BlogApplication.Infrastructure.Data;

namespace BlogApplication.Infrastructure.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly MongoDbContext _context;

    public BlogRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Blog?> GetByIdAsync(string id)
    {
        return await _context.Blogs
            .Find(b => b.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Blog>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _context.Blogs
            .Find(_ => true)
            .SortByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Blog>> GetByUserIdAsync(int userId, int pageNumber, int pageSize)
    {
        return await _context.Blogs
            .Find(b => b.UserId == userId)
            .SortByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Blog>> GetPublishedAsync(int pageNumber, int pageSize)
    {
        return await _context.Blogs
            .Find(b => b.IsPublished)
            .SortByDescending(b => b.PublishedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Blog>> SearchAsync(string searchTerm, int pageNumber, int pageSize)
    {
        var filter = Builders<Blog>.Filter.Or(
            Builders<Blog>.Filter.Regex(b => b.Title, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Blog>.Filter.Regex(b => b.Content, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Blog>.Filter.AnyIn(b => b.Tags, new[] { searchTerm })
        );

        return await _context.Blogs
            .Find(filter)
            .SortByDescending(b => b.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<Blog> CreateAsync(Blog blog)
    {
        await _context.Blogs.InsertOneAsync(blog);
        return blog;
    }

    public async Task UpdateAsync(Blog blog)
    {
        await _context.Blogs.ReplaceOneAsync(b => b.Id == blog.Id, blog);
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Blogs.DeleteOneAsync(b => b.Id == id);
    }

    public async Task<long> GetTotalCountAsync()
    {
        return await _context.Blogs.CountDocumentsAsync(_ => true);
    }

    public async Task<long> GetUserBlogsCountAsync(int userId)
    {
        return await _context.Blogs.CountDocumentsAsync(b => b.UserId == userId);
    }

    public async Task IncrementViewCountAsync(string id)
    {
        var update = Builders<Blog>.Update.Inc(b => b.ViewCount, 1);
        await _context.Blogs.UpdateOneAsync(b => b.Id == id, update);
    }
}

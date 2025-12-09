using BlogApplication.Domain.Entities;

namespace BlogApplication.Domain.Interfaces;

/// <summary>
/// Repository interface for blog data access operations in MongoDB.
/// </summary>
public interface IBlogRepository
{
    Task<Blog?> GetByIdAsync(string id);
    Task<IEnumerable<Blog>> GetAllAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Blog>> GetByUserIdAsync(int userId, int pageNumber, int pageSize);
    Task<IEnumerable<Blog>> GetPublishedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Blog>> GetPublishedByCategoryAsync(int categoryId, int pageNumber, int pageSize);
    Task<IEnumerable<Blog>> SearchAsync(string searchTerm, int pageNumber, int pageSize);
    Task<Blog> CreateAsync(Blog blog);
    Task UpdateAsync(Blog blog);
    Task DeleteAsync(string id);
    Task<long> GetTotalCountAsync();
    Task<long> GetUserBlogsCountAsync(int userId);
    Task<long> GetCategoryBlogsCountAsync(int categoryId);
    Task IncrementViewCountAsync(string id);
}

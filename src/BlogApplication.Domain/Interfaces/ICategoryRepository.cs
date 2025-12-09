using BlogApplication.Domain.Entities;

namespace BlogApplication.Domain.Interfaces;

/// <summary>
/// Repository interface for category data access operations.
/// </summary>
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<IEnumerable<Category>> GetActiveAsync();
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
}

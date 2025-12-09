using BlogApplication.Domain.Entities;

namespace BlogApplication.Domain.Interfaces;

/// <summary>
/// Repository interface for role data access operations.
/// </summary>
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> CreateAsync(Role role);
}

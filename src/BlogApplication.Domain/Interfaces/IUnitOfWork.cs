namespace BlogApplication.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern implementation for managing database transactions.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    Task<int> SaveChangesAsync();
}

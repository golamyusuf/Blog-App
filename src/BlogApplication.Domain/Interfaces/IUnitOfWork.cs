namespace BlogApplication.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    Task<int> SaveChangesAsync();
}

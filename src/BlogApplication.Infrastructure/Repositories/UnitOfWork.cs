using BlogApplication.Domain.Interfaces;
using BlogApplication.Infrastructure.Data;

namespace BlogApplication.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }

    public UnitOfWork(ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _context = context;
        Users = userRepository;
        Roles = roleRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

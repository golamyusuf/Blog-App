using BlogApplication.Domain.Entities;
using BlogApplication.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BlogApplication.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAdminUser(ApplicationDbContext context)
    {
        // Check if admin user already exists
        var adminExists = await context.Users
            .AnyAsync(u => u.Email == "admin@blogapp.com");

        if (!adminExists)
        {
            var passwordHasher = new PasswordHasher();
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@blogapp.com",
                PasswordHash = passwordHasher.HashPassword("Admin@123"), // Change this password!
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            // Assign Admin role
            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.UtcNow
                };
                context.UserRoles.Add(userRole);
                await context.SaveChangesAsync();
            }

            // Also assign User role
            var userRole2 = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "User");

            if (userRole2 != null)
            {
                var userRoleAssignment = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = userRole2.Id,
                    AssignedAt = DateTime.UtcNow
                };
                context.UserRoles.Add(userRoleAssignment);
                await context.SaveChangesAsync();
            }
        }
    }
}

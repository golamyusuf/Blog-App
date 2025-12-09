using BlogApplication.Domain.Entities;

namespace BlogApplication.Domain.Interfaces;

/// <summary>
/// Repository interface for user data access operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByIdAsync(int id);
    
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByUsernameAsync(string username);
    
    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>A collection of all users.</returns>
    Task<IEnumerable<User>> GetAllAsync();
    
    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>The created user with generated ID.</returns>
    Task<User> CreateAsync(User user);
    
    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="user">The user with updated information.</param>
    Task UpdateAsync(User user);
    
    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    Task DeleteAsync(int id);
    
    /// <summary>
    /// Checks if a user exists with the specified email or username.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="username">The username to check.</param>
    /// <returns>True if a user exists, otherwise false.</returns>
    Task<bool> ExistsAsync(string email, string username);
}

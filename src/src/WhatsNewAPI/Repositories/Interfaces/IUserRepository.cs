using WhatsNewAPI.Models.Entities;

namespace WhatsNewAPI.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task UpdateLastLoginAsync(int userId);
}

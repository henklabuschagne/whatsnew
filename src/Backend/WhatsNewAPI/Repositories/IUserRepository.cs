using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhatsNewAPI.Models;

namespace WhatsNewAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
    }
}

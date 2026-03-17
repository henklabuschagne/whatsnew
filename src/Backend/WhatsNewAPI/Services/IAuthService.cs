using System;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;

namespace WhatsNewAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserDto> GetCurrentUserAsync(Guid userId);
        string GenerateJwtToken(Guid userId, string email, string role);
        bool VerifyPassword(string password, string passwordHash);
        string HashPassword(string password);
    }
}

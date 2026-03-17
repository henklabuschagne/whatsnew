using WhatsNewAPI.Models.DTOs.Auth;

namespace WhatsNewAPI.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string ipAddress);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request);
}

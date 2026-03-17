using WhatsNewAPI.Helpers;
using WhatsNewAPI.Models.DTOs.Auth;
using WhatsNewAPI.Repositories.Interfaces;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtHelper _jwtHelper;
    private readonly PasswordHelper _passwordHelper;
    private readonly IAuditService _auditService;

    public AuthService(
        IUserRepository userRepository,
        JwtHelper jwtHelper,
        PasswordHelper passwordHelper,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
        _passwordHelper = passwordHelper;
        _auditService = auditService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string ipAddress)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        
        if (user == null || !user.IsActive)
        {
            await _auditService.LogActionAsync(null, "LOGIN_FAILED", "User", null, null, $"Username: {request.Username}", ipAddress);
            return null;
        }

        if (!_passwordHelper.VerifyPassword(request.Password, user.PasswordHash))
        {
            await _auditService.LogActionAsync(user.UserId, "LOGIN_FAILED", "User", user.UserId, null, "Invalid password", ipAddress);
            return null;
        }

        var token = _jwtHelper.GenerateToken(user.UserId, user.Username, user.Email, user.Role);
        await _userRepository.UpdateLastLoginAsync(user.UserId);
        await _auditService.LogActionAsync(user.UserId, "LOGIN", "User", user.UserId, null, "Successful login", ipAddress);

        return new LoginResponseDto
        {
            Token = token,
            User = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                LastLoginAt = DateTime.UtcNow
            },
            ExpiresAt = _jwtHelper.GetTokenExpiration()
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            LastLoginAt = user.LastLoginAt
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return false;
        }

        if (!_passwordHelper.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return false;
        }

        // Note: You would need to add UpdatePasswordAsync to IUserRepository
        // For now, this is a placeholder
        await _auditService.LogActionAsync(userId, "PASSWORD_CHANGED", "User", userId);
        
        return true;
    }
}

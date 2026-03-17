// ============================================
// What's New API - Interfaces
// Repository and Service Interfaces
// ============================================

using WhatsNewAPI.Models;
using WhatsNewAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace WhatsNewAPI.Repositories.Interfaces
{
    // ============================================
    // REPOSITORY INTERFACES
    // ============================================

    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> UpdatePasswordAsync(int userId, string passwordHash);
        Task<bool> DeleteAsync(int userId);
        Task UpdateLastLoginAsync(int userId);
    }

    public interface IReleaseRepository
    {
        Task<List<Release>> GetAllAsync(bool includeUnpublished = false);
        Task<Release> GetByIdAsync(int releaseId);
        Task<Release> GetByVersionAsync(string version);
        Task<int> CreateAsync(Release release, int createdBy);
        Task<bool> UpdateAsync(int releaseId, Release release, int updatedBy);
        Task<bool> DeleteAsync(int releaseId, int deletedBy);
        Task<bool> PublishAsync(int releaseId, bool isPublished, int updatedBy);
        Task<int> GetChangeCountAsync(int releaseId);
    }

    public interface IChangeRepository
    {
        Task<Change> GetByIdAsync(int changeId);
        Task<List<Change>> GetByReleaseIdAsync(int releaseId);
        Task<List<Change>> GetByChangeTypeAsync(string changeType);
        Task<int> CreateAsync(Change change, List<string> moduleTags, int createdBy);
        Task<bool> UpdateAsync(int changeId, Change change, List<string> moduleTags, int updatedBy);
        Task<bool> DeleteAsync(int changeId, int deletedBy);
        Task<List<Change>> SearchAsync(string searchTerm);
    }

    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync(bool activeOnly = true);
        Task<Tag> GetByIdAsync(int tagId);
        Task<Tag> GetByValueAsync(string tagValue);
        Task<int> CreateAsync(Tag tag, int createdBy);
        Task<bool> UpdateAsync(int tagId, Tag tag, int updatedBy);
        Task<bool> DeleteAsync(int tagId, int deletedBy);
        Task<bool> ExistsAsync(string tagValue);
        Task<int> GetUsageCountAsync(int tagId);
    }

    public interface IAuditRepository
    {
        Task LogAsync(AuditLog auditLog);
        Task<List<AuditLog>> GetLogsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? userId = null,
            string entityType = null,
            int pageNumber = 1,
            int pageSize = 50);
        Task<int> GetTotalCountAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? userId = null,
            string entityType = null);
    }
}

namespace WhatsNewAPI.Services.Interfaces
{
    using WhatsNewAPI.Models.DTOs.Auth;
    using WhatsNewAPI.Models.DTOs.Releases;
    using WhatsNewAPI.Models.DTOs.Changes;
    using WhatsNewAPI.Models.DTOs.Tags;
    using WhatsNewAPI.Models.DTOs.Common;

    // ============================================
    // SERVICE INTERFACES
    // ============================================

    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress);
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto request);
        Task<bool> ValidateTokenAsync(string token);
    }

    public interface IReleaseService
    {
        Task<List<ReleaseDto>> GetAllReleasesAsync(bool includeUnpublished = false);
        Task<ReleaseDetailDto> GetReleaseByIdAsync(int releaseId);
        Task<ReleaseDto> CreateReleaseAsync(CreateReleaseDto request, int userId);
        Task<bool> UpdateReleaseAsync(int releaseId, UpdateReleaseDto request, int userId);
        Task<bool> DeleteReleaseAsync(int releaseId, int userId);
        Task<bool> PublishReleaseAsync(int releaseId, bool isPublished, int userId);
        Task<StatisticsDto> GetStatisticsAsync();
        Task<ImportResultDto> ImportFromExcelAsync(IFormFile file, int userId);
        Task<byte[]> ExportToExcelAsync(bool includeUnpublished = true);
    }

    public interface IChangeService
    {
        Task<ChangeDto> GetByIdAsync(int changeId);
        Task<List<ChangeDto>> GetByReleaseIdAsync(int releaseId);
        Task<ChangeDto> CreateChangeAsync(CreateChangeDto request, int userId);
        Task<bool> UpdateChangeAsync(int changeId, UpdateChangeDto request, int userId);
        Task<bool> DeleteChangeAsync(int changeId, int userId);
        Task<List<ChangeDto>> SearchChangesAsync(string searchTerm);
    }

    public interface ITagService
    {
        Task<List<TagDto>> GetAllTagsAsync(bool activeOnly = true);
        Task<TagDto> GetByIdAsync(int tagId);
        Task<TagDto> CreateTagAsync(CreateTagDto request, int userId);
        Task<bool> UpdateTagAsync(int tagId, UpdateTagDto request, int userId);
        Task<bool> DeleteTagAsync(int tagId, int userId);
        Task<bool> TagExistsAsync(string tagValue);
    }

    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetByIdAsync(int userId);
        Task<UserDto> CreateUserAsync(CreateUserDto request, int createdBy);
        Task<bool> UpdateUserAsync(int userId, UpdateUserDto request, int updatedBy);
        Task<bool> DeleteUserAsync(int userId, int deletedBy);
    }

    public interface IAuditService
    {
        Task LogActionAsync(
            int? userId,
            string action,
            string entityType,
            int? entityId = null,
            string oldValue = null,
            string newValue = null,
            string ipAddress = null,
            string userAgent = null);
        Task<PaginatedResponse<AuditLog>> GetAuditLogsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? userId = null,
            string entityType = null,
            int pageNumber = 1,
            int pageSize = 50);
    }
}

namespace WhatsNewAPI.Models.DTOs.Users
{
    // Additional User DTOs for IUserService

    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }

    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}

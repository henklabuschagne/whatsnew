// ============================================
// What's New API - DTOs and Models
// ============================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhatsNewAPI.Models
{
    // ============================================
    // ENTITY MODELS (Database Entities)
    // ============================================

    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } // "admin" or "viewer"
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class Release
    {
        public int ReleaseId { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public List<Change> Changes { get; set; } = new();
        public string CreatedByUsername { get; set; }
    }

    public class Change
    {
        public int ChangeId { get; set; }
        public int ReleaseId { get; set; }
        public string Description { get; set; }
        public string ChangeType { get; set; } // "bug_fix", "new_feature", "enhancement"
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public List<Tag> Tags { get; set; } = new();
        public List<string> ModuleTags { get; set; } = new();
    }

    public class Tag
    {
        public int TagId { get; set; }
        public string TagValue { get; set; }
        public string TagLabel { get; set; }
        public string TagType { get; set; } // "module" or "custom"
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ChangeTag
    {
        public int ChangeId { get; set; }
        public int TagId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SQLIntegrationSetting
    {
        public int SettingId { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastSyncAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AuditLog
    {
        public long AuditId { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public int? EntityId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace WhatsNewAPI.Models.DTOs.Auth
{
    // ============================================
    // AUTHENTICATION DTOs
    // ============================================

    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; }
    }
}

namespace WhatsNewAPI.Models.DTOs.Releases
{
    // ============================================
    // RELEASE DTOs
    // ============================================

    public class ReleaseDto
    {
        public int ReleaseId { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public int ChangeCount { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ReleaseDetailDto
    {
        public int ReleaseId { get; set; }
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public List<ChangeDto> Changes { get; set; } = new();
        public string CreatedByUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateReleaseDto
    {
        [Required(ErrorMessage = "Version is required")]
        [StringLength(50, ErrorMessage = "Version cannot exceed 50 characters")]
        public string Version { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        public DateTime ReleaseDate { get; set; }

        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
        public string Description { get; set; }

        public bool IsPublished { get; set; } = false;
    }

    public class UpdateReleaseDto
    {
        [Required(ErrorMessage = "Version is required")]
        [StringLength(50, ErrorMessage = "Version cannot exceed 50 characters")]
        public string Version { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        public DateTime ReleaseDate { get; set; }

        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
        public string Description { get; set; }

        public bool IsPublished { get; set; }
    }
}

namespace WhatsNewAPI.Models.DTOs.Changes
{
    // ============================================
    // CHANGE DTOs
    // ============================================

    public class ChangeDto
    {
        public int ChangeId { get; set; }
        public int ReleaseId { get; set; }
        public string Description { get; set; }
        public string ChangeType { get; set; }
        public List<string> ModuleTags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateChangeDto
    {
        [Required(ErrorMessage = "Release ID is required")]
        public int ReleaseId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Change type is required")]
        [RegularExpression("^(bug_fix|new_feature|enhancement)$", 
            ErrorMessage = "Change type must be bug_fix, new_feature, or enhancement")]
        public string ChangeType { get; set; }

        public List<string> ModuleTags { get; set; } = new();
    }

    public class UpdateChangeDto
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Change type is required")]
        [RegularExpression("^(bug_fix|new_feature|enhancement)$", 
            ErrorMessage = "Change type must be bug_fix, new_feature, or enhancement")]
        public string ChangeType { get; set; }

        public List<string> ModuleTags { get; set; } = new();
    }
}

namespace WhatsNewAPI.Models.DTOs.Tags
{
    // ============================================
    // TAG DTOs
    // ============================================

    public class TagDto
    {
        public int TagId { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateTagDto
    {
        [Required(ErrorMessage = "Tag value is required")]
        [StringLength(100, ErrorMessage = "Tag value cannot exceed 100 characters")]
        [RegularExpression("^[a-z_]+$", 
            ErrorMessage = "Tag value must contain only lowercase letters and underscores")]
        public string Value { get; set; }

        [Required(ErrorMessage = "Tag label is required")]
        [StringLength(100, ErrorMessage = "Tag label cannot exceed 100 characters")]
        public string Label { get; set; }

        public string Type { get; set; } = "module";
    }

    public class UpdateTagDto
    {
        [Required(ErrorMessage = "Tag label is required")]
        [StringLength(100, ErrorMessage = "Tag label cannot exceed 100 characters")]
        public string Label { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

namespace WhatsNewAPI.Models.DTOs.Common
{
    // ============================================
    // COMMON DTOs
    // ============================================

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Details { get; set; }
        public List<string> Errors { get; set; } = new();
        public string TraceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class ImportResultDto
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    public class StatisticsDto
    {
        public int TotalReleases { get; set; }
        public int PublishedReleases { get; set; }
        public int TotalChanges { get; set; }
        public int BugFixes { get; set; }
        public int NewFeatures { get; set; }
        public int Enhancements { get; set; }
        public List<ModuleStatDto> ModuleStats { get; set; } = new();
    }

    public class ModuleStatDto
    {
        public string ModuleName { get; set; }
        public int ChangeCount { get; set; }
    }
}

namespace WhatsNewAPI.Models.DTOs.Excel
{
    // ============================================
    // EXCEL IMPORT/EXPORT DTOs
    // ============================================

    public class ExcelReleaseDto
    {
        public string Version { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public string ChangeType { get; set; }
        public string ChangeDescription { get; set; }
        public string ModuleTags { get; set; } // Comma-separated
    }

    public class ExcelImportOptions
    {
        public bool SkipFirstRow { get; set; } = true;
        public bool ValidateData { get; set; } = true;
        public bool CreateMissingTags { get; set; } = false;
    }
}

// ============================================
// What's New API - Controllers
// ============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WhatsNewAPI.Models.DTOs;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Controllers
{
    // ============================================
    // AUTH CONTROLLER
    // ============================================

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await _authService.LoginAsync(request, GetClientIp());
                
                if (response == null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid username or password"));
                }

                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for username: {Username}", request.Username);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred during login"));
            }
        }

        /// <summary>
        /// Get current user info
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _authService.ChangePasswordAsync(userId, request);

                if (!success)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Failed to change password. Check your current password."));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Password changed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }

        private string GetClientIp()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    // ============================================
    // RELEASES CONTROLLER
    // ============================================

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReleasesController : ControllerBase
    {
        private readonly IReleaseService _releaseService;
        private readonly ILogger<ReleasesController> _logger;

        public ReleasesController(IReleaseService releaseService, ILogger<ReleasesController> logger)
        {
            _releaseService = releaseService;
            _logger = logger;
        }

        /// <summary>
        /// Get all releases (viewers see published only, admins see all)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ReleaseDto>>), 200)]
        public async Task<IActionResult> GetAllReleases([FromQuery] bool includeUnpublished = false)
        {
            try
            {
                var userRole = GetCurrentUserRole();
                
                // Viewers can only see published releases
                if (userRole != "admin")
                {
                    includeUnpublished = false;
                }

                var releases = await _releaseService.GetAllReleasesAsync(includeUnpublished);
                return Ok(ApiResponse<List<ReleaseDto>>.SuccessResponse(releases));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting releases");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Get release by ID with all changes
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ReleaseDetailDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReleaseById(int id)
        {
            try
            {
                var release = await _releaseService.GetReleaseByIdAsync(id);

                if (release == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Release not found"));
                }

                // Check if user has permission to view unpublished releases
                var userRole = GetCurrentUserRole();
                if (!release.IsPublished && userRole != "admin")
                {
                    return Forbid();
                }

                return Ok(ApiResponse<ReleaseDetailDto>.SuccessResponse(release));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting release {ReleaseId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Create new release (admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<ReleaseDto>), 201)]
        public async Task<IActionResult> CreateRelease([FromBody] CreateReleaseDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var release = await _releaseService.CreateReleaseAsync(request, userId);

                return CreatedAtAction(
                    nameof(GetReleaseById),
                    new { id = release.ReleaseId },
                    ApiResponse<ReleaseDto>.SuccessResponse(release, "Release created successfully")
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating release");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Update release (admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdateRelease(int id, [FromBody] UpdateReleaseDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _releaseService.UpdateReleaseAsync(id, request, userId);

                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Release not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Release updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating release {ReleaseId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Delete release (admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> DeleteRelease(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _releaseService.DeleteReleaseAsync(id, userId);

                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Release not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Release deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting release {ReleaseId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Get release statistics (admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<StatisticsDto>), 200)]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var stats = await _releaseService.GetStatisticsAsync();
                return Ok(ApiResponse<StatisticsDto>.SuccessResponse(stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Import releases from Excel (admin only)
        /// </summary>
        [HttpPost("import/excel")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<ImportResultDto>), 200)]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("No file uploaded"));
                }

                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid file format. Only Excel files are allowed."));
                }

                var userId = GetCurrentUserId();
                var result = await _releaseService.ImportFromExcelAsync(file, userId);

                return Ok(ApiResponse<ImportResultDto>.SuccessResponse(result, "Import completed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing from Excel");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred during import"));
            }
        }

        /// <summary>
        /// Export releases to Excel (admin only)
        /// </summary>
        [HttpGet("export/excel")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ExportToExcel([FromQuery] bool includeUnpublished = true)
        {
            try
            {
                var fileBytes = await _releaseService.ExportToExcelAsync(includeUnpublished);
                var fileName = $"releases_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to Excel");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred during export"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "viewer";
        }
    }

    // ============================================
    // CHANGES CONTROLLER
    // ============================================

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class ChangesController : ControllerBase
    {
        private readonly IChangeService _changeService;
        private readonly ILogger<ChangesController> _logger;

        public ChangesController(IChangeService changeService, ILogger<ChangesController> logger)
        {
            _changeService = changeService;
            _logger = logger;
        }

        /// <summary>
        /// Create new change
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ChangeDto>), 201)]
        public async Task<IActionResult> CreateChange([FromBody] CreateChangeDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var change = await _changeService.CreateChangeAsync(request, userId);

                return Ok(ApiResponse<ChangeDto>.SuccessResponse(change, "Change created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating change");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Update change
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdateChange(int id, [FromBody] UpdateChangeDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _changeService.UpdateChangeAsync(id, request, userId);

                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Change not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Change updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating change {ChangeId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Delete change
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> DeleteChange(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _changeService.DeleteChangeAsync(id, userId);

                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Change not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Change deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting change {ChangeId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }
    }

    // ============================================
    // TAGS CONTROLLER
    // ============================================

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagsController> _logger;

        public TagsController(ITagService tagService, ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tags
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<TagDto>>), 200)]
        public async Task<IActionResult> GetAllTags([FromQuery] bool activeOnly = true)
        {
            try
            {
                var tags = await _tagService.GetAllTagsAsync(activeOnly);
                return Ok(ApiResponse<List<TagDto>>.SuccessResponse(tags));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tags");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Create tag (admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<TagDto>), 201)]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tag = await _tagService.CreateTagAsync(request, userId);

                return Ok(ApiResponse<TagDto>.SuccessResponse(tag, "Tag created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Update tag (admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _tagService.UpdateTagAsync(id, request, userId);

                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Tag not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Tag updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag {TagId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        /// <summary>
        /// Delete tag (admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _tagService.DeleteTagAsync(id, userId);

                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Tag not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Tag deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag {TagId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred"));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }
    }
}

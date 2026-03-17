using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WhatsNewAPI.Models.DTOs.Releases;
using WhatsNewAPI.Models.DTOs.Common;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Controllers;

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

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "viewer";
    }
}

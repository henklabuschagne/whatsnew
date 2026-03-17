using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WhatsNewAPI.Models.DTOs.Tags;
using WhatsNewAPI.Models.DTOs.Common;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Controllers;

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
        return int.Parse(userIdClaim ?? "0");
    }
}

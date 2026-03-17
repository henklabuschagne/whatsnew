using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WhatsNewAPI.Models.DTOs.Changes;
using WhatsNewAPI.Models.DTOs.Common;
using WhatsNewAPI.Services.Interfaces;

namespace WhatsNewAPI.Controllers;

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
        return int.Parse(userIdClaim ?? "0");
    }
}

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Repositories;

namespace WhatsNewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReleaseNotesController : ControllerBase
    {
        private readonly IReleaseNoteRepository _releaseNoteRepository;
        private const long MaxFileSize = 52428800; // 50MB
        private static readonly string[] AllowedExtensions = { 
            ".pdf", ".doc", ".docx", ".txt", ".md", 
            ".png", ".jpg", ".jpeg", ".gif", 
            ".xlsx", ".xls", ".pptx", ".ppt" 
        };

        public ReleaseNotesController(IReleaseNoteRepository releaseNoteRepository)
        {
            _releaseNoteRepository = releaseNoteRepository;
        }

        /// <summary>
        /// Get all release notes for a specific change
        /// </summary>
        [HttpGet("change/{changeId}")]
        public async Task<IActionResult> GetReleaseNotesByChangeId(Guid changeId)
        {
            try
            {
                var releaseNotes = await _releaseNoteRepository.GetReleaseNotesByChangeIdAsync(changeId);
                return Ok(releaseNotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving release notes", error = ex.Message });
            }
        }

        /// <summary>
        /// Get count of release notes for a change
        /// </summary>
        [HttpGet("change/{changeId}/count")]
        public async Task<IActionResult> GetReleaseNotesCount(Guid changeId)
        {
            try
            {
                var count = await _releaseNoteRepository.GetReleaseNotesCountAsync(changeId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while counting release notes", error = ex.Message });
            }
        }

        /// <summary>
        /// Download a release note file
        /// </summary>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadReleaseNote(Guid id)
        {
            try
            {
                var releaseNote = await _releaseNoteRepository.GetReleaseNoteByIdAsync(id);
                
                if (releaseNote == null)
                    return NotFound(new { message = "Release note not found" });

                return File(releaseNote.FileData, releaseNote.FileType, releaseNote.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while downloading the file", error = ex.Message });
            }
        }

        /// <summary>
        /// Upload a release note file
        /// </summary>
        [HttpPost("upload")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UploadReleaseNote(
            [FromForm] Guid changeId,
            [FromForm] IFormFile file)
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                // Check file size
                if (file.Length > MaxFileSize)
                {
                    return BadRequest(new { message = $"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024}MB" });
                }

                // Check file extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = $"File type not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}" });
                }

                // Read file data
                byte[] fileData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                }

                // Get current user ID (optional)
                Guid? userId = null;
                var userIdClaim = User.FindFirst("userId");
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                // Create DTO
                var createDto = new CreateReleaseNoteDto
                {
                    ChangeId = changeId,
                    FileName = file.FileName,
                    FileSize = file.Length,
                    FileType = file.ContentType,
                    FileExtension = extension,
                    FileData = fileData,
                    UploadedBy = userId
                };

                // Save to database
                var createdNote = await _releaseNoteRepository.CreateReleaseNoteAsync(createDto);

                if (createdNote == null)
                {
                    return StatusCode(500, new { message = "Failed to upload release note" });
                }

                return Ok(new ReleaseNoteUploadResponseDto
                {
                    Success = true,
                    Message = "File uploaded successfully",
                    ReleaseNote = createdNote
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while uploading the file", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a release note
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteReleaseNote(Guid id)
        {
            try
            {
                var success = await _releaseNoteRepository.DeleteReleaseNoteAsync(id);

                if (!success)
                    return NotFound(new { message = "Release note not found" });

                return Ok(new { message = "Release note deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the release note", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all release notes (admin only)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllReleaseNotes([FromQuery] int topN = 100)
        {
            try
            {
                var releaseNotes = await _releaseNoteRepository.GetAllReleaseNotesAsync(topN);
                return Ok(releaseNotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving release notes", error = ex.Message });
            }
        }
    }
}

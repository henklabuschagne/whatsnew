using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsNewAPI.DTOs;
using WhatsNewAPI.Models;
using WhatsNewAPI.Repositories;

namespace WhatsNewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChangesController : ControllerBase
    {
        private readonly IChangeRepository _changeRepository;
        private readonly ITagRepository _tagRepository;

        public ChangesController(IChangeRepository changeRepository, ITagRepository tagRepository)
        {
            _changeRepository = changeRepository;
            _tagRepository = tagRepository;
        }

        [HttpGet("release/{releaseId}")]
        public async Task<IActionResult> GetChangesByReleaseId(Guid releaseId)
        {
            try
            {
                var changes = await _changeRepository.GetChangesByReleaseIdAsync(releaseId);
                var allTags = await _tagRepository.GetAllTagsAsync();
                var tagDict = allTags.ToDictionary(t => t.TagId, t => t);

                var changeDtos = changes.Select(c => new ChangeDto
                {
                    ChangeId = c.ChangeId,
                    ReleaseId = c.ReleaseId,
                    Description = c.Description,
                    ChangeType = c.ChangeType,
                    CreatedAt = c.CreatedAt,
                    TagIds = c.TagIds,
                    ModuleTags = c.TagIds
                        .Where(tagId => tagDict.ContainsKey(tagId))
                        .Select(tagId => tagDict[tagId].Value)
                        .ToList()
                });

                return Ok(changeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving changes", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChangeById(Guid id)
        {
            try
            {
                var change = await _changeRepository.GetChangeByIdAsync(id);

                if (change == null)
                {
                    return NotFound(new { message = "Change not found" });
                }

                var allTags = await _tagRepository.GetAllTagsAsync();
                var tagDict = allTags.ToDictionary(t => t.TagId, t => t);

                var changeDto = new ChangeDto
                {
                    ChangeId = change.ChangeId,
                    ReleaseId = change.ReleaseId,
                    Description = change.Description,
                    ChangeType = change.ChangeType,
                    CreatedAt = change.CreatedAt,
                    TagIds = change.TagIds,
                    ModuleTags = change.TagIds
                        .Where(tagId => tagDict.ContainsKey(tagId))
                        .Select(tagId => tagDict[tagId].Value)
                        .ToList()
                };

                return Ok(changeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the change", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateChange([FromBody] CreateChangeDto createChangeDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createChangeDto.Description))
                {
                    return BadRequest(new { message = "Description is required" });
                }

                var change = new Change
                {
                    ReleaseId = createChangeDto.ReleaseId,
                    Description = createChangeDto.Description,
                    ChangeType = createChangeDto.ChangeType,
                    TagIds = createChangeDto.TagIds ?? new System.Collections.Generic.List<Guid>()
                };

                var createdChange = await _changeRepository.CreateChangeAsync(change);

                if (createdChange == null)
                {
                    return BadRequest(new { message = "Failed to create change" });
                }

                var allTags = await _tagRepository.GetAllTagsAsync();
                var tagDict = allTags.ToDictionary(t => t.TagId, t => t);

                var changeDto = new ChangeDto
                {
                    ChangeId = createdChange.ChangeId,
                    ReleaseId = createdChange.ReleaseId,
                    Description = createdChange.Description,
                    ChangeType = createdChange.ChangeType,
                    CreatedAt = createdChange.CreatedAt,
                    TagIds = createdChange.TagIds,
                    ModuleTags = createdChange.TagIds
                        .Where(tagId => tagDict.ContainsKey(tagId))
                        .Select(tagId => tagDict[tagId].Value)
                        .ToList()
                };

                return CreatedAtAction(nameof(GetChangeById), new { id = changeDto.ChangeId }, changeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the change", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateChange(Guid id, [FromBody] UpdateChangeDto updateChangeDto)
        {
            try
            {
                if (string.IsNullOrEmpty(updateChangeDto.Description))
                {
                    return BadRequest(new { message = "Description is required" });
                }

                var change = new Change
                {
                    ChangeId = id,
                    Description = updateChangeDto.Description,
                    ChangeType = updateChangeDto.ChangeType,
                    TagIds = updateChangeDto.TagIds ?? new System.Collections.Generic.List<Guid>()
                };

                var updatedChange = await _changeRepository.UpdateChangeAsync(change);

                if (updatedChange == null)
                {
                    return NotFound(new { message = "Change not found" });
                }

                var allTags = await _tagRepository.GetAllTagsAsync();
                var tagDict = allTags.ToDictionary(t => t.TagId, t => t);

                var changeDto = new ChangeDto
                {
                    ChangeId = updatedChange.ChangeId,
                    ReleaseId = updatedChange.ReleaseId,
                    Description = updatedChange.Description,
                    ChangeType = updatedChange.ChangeType,
                    CreatedAt = updatedChange.CreatedAt,
                    TagIds = updatedChange.TagIds,
                    ModuleTags = updatedChange.TagIds
                        .Where(tagId => tagDict.ContainsKey(tagId))
                        .Select(tagId => tagDict[tagId].Value)
                        .ToList()
                };

                return Ok(changeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the change", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteChange(Guid id)
        {
            try
            {
                var success = await _changeRepository.DeleteChangeAsync(id);

                if (!success)
                {
                    return NotFound(new { message = "Change not found" });
                }

                return Ok(new { message = "Change deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the change", error = ex.Message });
            }
        }
    }
}
